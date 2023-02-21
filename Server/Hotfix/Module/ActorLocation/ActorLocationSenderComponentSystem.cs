﻿using System;
using System.IO;
namespace ET {
    [ObjectSystem]
    public class ActorLocationSenderComponentAwakeSystem: AwakeSystem<ActorLocationSenderComponent> {
        public override void Awake(ActorLocationSenderComponent self) {
            ActorLocationSenderComponent.Instance = self;
            // 每10s扫描一次过期的actorproxy进行回收,过期时间是2分钟
            // 可能由于bug或者进程挂掉，导致ActorLocationSender发送的消息没有确认，结果无法自动删除，每一分钟清理一次这种ActorLocationSender
            self.CheckTimer = TimerComponent.Instance.NewRepeatedTimer(10 * 1000, self.Check);
        }
    }
    [ObjectSystem]
    public class ActorLocationSenderComponentDestroySystem: DestroySystem<ActorLocationSenderComponent> {
        public override void Destroy(ActorLocationSenderComponent self) {
            ActorLocationSenderComponent.Instance = null;
            TimerComponent.Instance.Remove(ref self.CheckTimer);
        }
    }
// 位置发送组件系统：是谁在使用这个系统，是只有服务器端吗？好像是
    public static class ActorLocationSenderComponentSystem {
        public static void Check(this ActorLocationSenderComponent self) { // 单例模式
            using (ListComponent<long> list = ListComponent<long>.Create()) { // 这里，用来加入，超时了的位置请求消息
                long timeNow = TimeHelper.ServerNow();
                foreach ((long key, Entity value) in self.Children) {
                    ActorLocationSender actorLocationMessageSender = (ActorLocationSender) value;
                    if (timeNow > actorLocationMessageSender.LastSendOrRecvTime + ActorLocationSenderComponent.TIMEOUT_TIME) {
                        list.List.Add(key); // 这里，用来加入，超时了的位置请求消息
                    }
                }
                foreach (long id in list.List) {
// 超时了的，是自动移除了。前面自己看的时候不是说，有的小伙伴搬家的时候，只要搬家前申明过，就不会丢失消息吗？还是说它搬家的过程时间过长？
                    self.Remove(id); 
                }
            }
        }
// 当前内部消息：字典里存有，就取一个；没有创建一个
        private static ActorLocationSender GetOrCreate(this ActorLocationSenderComponent self, long id) {
            if (id == 0) {
                throw new Exception($"actor id is 0");
            }
            if (self.Children.TryGetValue(id, out Entity actorLocationSender)) { // 大概是 InstanceId,entityId
                return (ActorLocationSender) actorLocationSender;
            }
            actorLocationSender = self.AddChildWithId<ActorLocationSender>(id); // 将传入的Unit当作actorLocationSender的组件ID
            // 上面一行：这个框架，层层封装比较多，很多逻辑封装在框架的最底层。比如上面，同时缓存到ActorLocationSenderComponent组件的ActorLocationSenders字典中，方便下次直接找到这个actorLocationSender 也封装在底层，甚至作了缓存数据库的准备（这块儿没有细看）
            return (ActorLocationSender) actorLocationSender;
        }
        private static void Remove(this ActorLocationSenderComponent self, long id) {
            if (!self.Children.TryGetValue(id, out Entity actorMessageSender)) {
                return;
            }
            actorMessageSender.Dispose(); // 这里只移除了值 
        }
        public static void Send(this ActorLocationSenderComponent self, long entityId, IActorRequest message) {
            self.Call(entityId, message).Coroutine(); // 发消息：调用下面内部，异步调用方法 
        }
        // 获取ActorLocationSenderComponent组件，通过Get方法获取一个ActorLocationSender，这【这里Call()】是个异步函数
        public static async ETTask<IActorResponse> Call(this ActorLocationSenderComponent self, long entityId, IActorRequest iActorRequest) {
            ActorLocationSender actorLocationSender = self.GetOrCreate(entityId);
            // 先序列化好
            int rpcId = ActorMessageSenderComponent.Instance.GetRpcId();
            iActorRequest.RpcId = rpcId;
            (ushort _, MemoryStream stream) = MessageSerializeHelper.MessageToStream(0, iActorRequest);
            
            long actorLocationSenderInstanceId = actorLocationSender.InstanceId;
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorLocationSender, entityId)) {
// 通过CoroutineLockComponent组件的单例异步方法Wait(long key)查看，针对每个actorLocationSender的ID，会有一个LockQueue队列，结合ETBOOK知道这里使用了锁的机制。
    // 如果有锁，那么就代表请求的对象正在转移（小伙伴在搬家），即他的InstanceID正在发生变化，这里会分两种处理方式。
        // 当没有锁的时候，说明对象并没有转移，创建好队列，方便下次使用外，就直接返回；如果有锁，那么就需要等待转移成功，所以返回一个ETTaskCompletionSource，进行异步处理。
    // 如果转移成功或者转移超时，那么就会释放这个CoroutineLock锁，这时会派发解锁消息，这样CoroutineLockComponent就可以设置异步完成，从而让所有异步await async ETTask Wait(long key)的地方继续执行下去。
                if (actorLocationSender.InstanceId != actorLocationSenderInstanceId) {
                    throw new RpcException(ErrorCode.ERR_ActorTimeout, $"{stream.ToActorMessage()}");
                }
                // 队列中没处理的消息返回跟上个消息一样的报错
                if (actorLocationSender.Error == ErrorCode.ERR_NotFoundActor) {
                    return ActorHelper.CreateResponse(iActorRequest, actorLocationSender.Error);
                }
                try {
                    return await self.CallInner(actorLocationSender, rpcId, stream);
                }
                catch (RpcException) {
                    self.Remove(actorLocationSender.Id);
                    throw;
                }
                catch (Exception e) {
                    self.Remove(actorLocationSender.Id);
                    throw new Exception($"{stream.ToActorMessage()}", e);
                }
            }
        }
        private static async ETTask<IActorResponse> CallInner(this ActorLocationSenderComponent self, ActorLocationSender actorLocationSender, int rpcId, MemoryStream memoryStream) {
            int failTimes = 0;
            long instanceId = actorLocationSender.InstanceId;
            actorLocationSender.LastSendOrRecvTime = TimeHelper.ServerNow();
            
            while (true) {
                if (actorLocationSender.ActorId == 0) {
                    // 异步调用LocationProxyComponent的Get方法。
                    // LocationProxyComponent的Get方法会发送一个协议到Location服务，即：new ObjectGetRequest() { Key = key }
                    actorLocationSender.ActorId = await LocationProxyComponent.Instance.Get(actorLocationSender.Id);
                    if (actorLocationSender.InstanceId != instanceId) {
                        throw new RpcException(ErrorCode.ERR_ActorLocationSenderTimeout2, $"{memoryStream.ToActorMessage()}");
                    }
                }
                if (actorLocationSender.ActorId == 0) {
                    IActorRequest iActorRequest = (IActorRequest)memoryStream.ToActorMessage();
                    return ActorHelper.CreateResponse(iActorRequest, ErrorCode.ERR_NotFoundActor);
                }
                // actorLocationSender内部就有与之关联的ActorId（其实他就是某个Entity最新的instanceId，拿到这个instanceId即可找到对应的IP与端口去发送数据）
                IActorResponse response = await ActorMessageSenderComponent.Instance.Call(actorLocationSender.ActorId, rpcId, memoryStream, false);
                if (actorLocationSender.InstanceId != instanceId) {
                    throw new RpcException(ErrorCode.ERR_ActorLocationSenderTimeout3, $"{memoryStream.ToActorMessage()}");
                }
                switch (response.Error) {
                    case ErrorCode.ERR_NotFoundActor: {
                        // 如果没找到Actor,重试
                        ++failTimes;
                        if (failTimes > 20) {
                            Log.Debug($"actor send message fail, actorid: {actorLocationSender.Id}");
                            actorLocationSender.Error = ErrorCode.ERR_NotFoundActor;
                            // 这里不能删除actor，要让后面等待发送的消息也返回ERR_NotFoundActor，直到超时删除
                            return response;
                        }
                        // 等待0.5s再发送
                        await TimerComponent.Instance.WaitAsync(500);
                        if (actorLocationSender.InstanceId != instanceId) {
                            throw new RpcException(ErrorCode.ERR_ActorLocationSenderTimeout4, $"{memoryStream.ToActorMessage()}");
                        }
                        actorLocationSender.ActorId = 0;
                        continue;
                    }
                    case ErrorCode.ERR_ActorNoMailBoxComponent:
                    case ErrorCode.ERR_ActorTimeout: {
                        throw new RpcException(response.Error, $"{memoryStream.ToActorMessage()}");
                    }
                }
                if (ErrorCode.IsRpcNeedThrowException(response.Error)) {
                    throw new RpcException(response.Error, $"Message: {response.Message} Request: {memoryStream.ToActorMessage()}");
                }
                return response;
            }
        }
    }
}