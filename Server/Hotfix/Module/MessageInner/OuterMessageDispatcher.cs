ing System;
using System.IO;
namespace ET {
    public class OuterMessageDispatcher: IMessageDispatcher {

        public void Dispatch(Session session, MemoryStream memoryStream) {
            ushort opcode = BitConverter.ToUInt16(memoryStream.GetBuffer(), Packet.KcpOpcodeIndex);
            Type type = OpcodeTypeComponent.Instance.GetType(opcode);
            object message = MessageSerializeHelper.DeserializeFrom(opcode, type, memoryStream);
            if (message is IResponse response) {
                session.OnRead(opcode, response);
                return;
            }
            OpcodeHelper.LogMsg(session.DomainZone(), opcode, message);
            
            DispatchAsync(session, opcode, message).Coroutine();
        }
// 这是个异步无返回值的方法，意味着可以异步await调用，但貌似没看到await调用的地方，走的是同步调用
        public async ETVoid DispatchAsync(Session session, ushort opcode, object message) { // 传入的参数是：一个session，一个协议号，一个协议数据
            // 根据消息接口判断是不是Actor消息，不同的接口做不同的处理
            switch (message) {
                // 这类消息主要是客户端向Map,Location等服务发送消息时，都是走的Gate转发，
                // 之前也提到了MAP 和 Location等服务发送消息时并没有外网组件，因此与Map等服务进行通讯时，都使用的Gate服务进行转发
                // ET可以分为多进程服务，当客户端向Map发消息时，他发送一个Actor消息，这样当Gate接收到这个消息后，通过Actor的方式向Map发消息并等待回复，然后再回复给客户端
                case IActorLocationRequest actorLocationRequest: { // gate session收到actor rpc消息，先向actor 发送rpc请求，再将请求结果返回客户端
                    // 获取客户端在Gate上已经登录过的Player上的UnitId（这个ID是通过在其他服上创建过Unit对象后，拿到的UnitId）
                    long unitId = session.GetComponent<SessionPlayerComponent>().Player.UnitId;
                    int rpcId = actorLocationRequest.RpcId; // 这里要保存客户端的rpcId
                    long instanceId = session.InstanceId;
                    IResponse response = await ActorLocationSenderComponent.Instance.Call(unitId, actorLocationRequest);
                    response.RpcId = rpcId;
                    // session可能已经断开了，所以这里需要判断
                    if (session.InstanceId == instanceId) {
                        session.Reply(response);
                    }
                    break;
                }
                case IActorLocationMessage actorLocationMessage: {
                    long unitId = session.GetComponent<SessionPlayerComponent>().Player.UnitId;
                    ActorLocationSenderComponent.Instance.Send(unitId, actorLocationMessage);
                    break;
                }
                case IActorRequest actorRequest: {  // 分发IActorRequest消息，目前没有用到，需要的自己添加
                    break;
                }
                case IActorMessage actorMessage: {  // 分发IActorMessage消息，目前没有用到，需要的自己添加
                    break;
                }
                default: {
                    // 非Actor消息
                    MessageDispatcherComponent.Instance.Handle(session, opcode, message);
                    break;
                }
            }
        }
    }
}
