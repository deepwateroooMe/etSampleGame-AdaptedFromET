using System;
using System.Collections.Generic;
namespace ET { // 这个例子，再理解一下：Model 不可热更新层，提供的是申明定义，具体的实现，实现逻辑放入 Hotfix 热更新域里，可实现热更新 

// Actor消息分发组件：包装管理不同消息类型的处理器，不是回调，是处理器
    public class ActorMessageDispatcherComponent: Entity {
        public static ActorMessageDispatcherComponent Instance;
        public readonly Dictionary<Type, IMActorHandler> ActorMessageHandlers = new Dictionary<Type, IMActorHandler>();
    }
}