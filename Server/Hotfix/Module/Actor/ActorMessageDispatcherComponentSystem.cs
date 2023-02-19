using System;
namespace ET { // 不是说，前面Model 模型里是定义，是不可热更新的；这里热更新域里，是实现，是可以热更新的
    [ObjectSystem]
    public class ActorMessageDispatcherComponentAwakeSystem: AwakeSystem<ActorMessageDispatcherComponent> {
        public override void Awake(ActorMessageDispatcherComponent self) {
            ActorMessageDispatcherComponent.Instance = self;
            self.Awake();
        }
    }
    [ObjectSystem]
    public class ActorMessageDispatcherComponentLoadSystem: LoadSystem<ActorMessageDispatcherComponent> {
        public override void Load(ActorMessageDispatcherComponent self) {
            self.Load();
        }
    }
    [ObjectSystem]
    public class ActorMessageDispatcherComponentDestroySystem: DestroySystem<ActorMessageDispatcherComponent> {
        public override void Destroy(ActorMessageDispatcherComponent self) {
            self.ActorMessageHandlers.Clear();
            ActorMessageDispatcherComponent.Instance = null;
        }
    }
// Actor消息分发组件
    public static class ActorMessageDispatcherComponentHelper {
        public static void Awake(this ActorMessageDispatcherComponent self) {
            self.Load();
        }
        public static void Load(this ActorMessageDispatcherComponent self) { // 消息回调的，加载管理系统
            self.ActorMessageHandlers.Clear();
            var types = Game.EventSystem.GetTypes(typeof (ActorMessageHandlerAttribute)); // 但凡注册过是，消息处理器标签的
            foreach (Type type in types) { // 管理、注册所有不同消息类型的回调：创建所有不同消息类型回调的实例
                object obj = Activator.CreateInstance(type); // 创建一个处理器的实例
                IMActorHandler imHandler = obj as IMActorHandler;
                if (imHandler == null) {
                    throw new Exception($"message handler not inherit IMActorHandler abstract class: {obj.GetType().FullName}");
                }
                Type messageType = imHandler.GetRequestType();
                self.ActorMessageHandlers.Add(messageType, imHandler); // 往字典里，添加类型的回调
            }
        }
        // 分发actor消息
        public static async ETTask Handle(
            this ActorMessageDispatcherComponent self, Entity entity, object message, Action<IActorResponse> reply) {
            if (!self.ActorMessageHandlers.TryGetValue(message.GetType(), out IMActorHandler handler)) {
                throw new Exception($"not found message handler: {message}");
            }
            await handler.Handle(entity, message, reply);
        }
        public static bool TryGetHandler(this ActorMessageDispatcherComponent self,Type type, out IMActorHandler actorHandler) {
            return self.ActorMessageHandlers.TryGetValue(type, out actorHandler);
        }
    }
}