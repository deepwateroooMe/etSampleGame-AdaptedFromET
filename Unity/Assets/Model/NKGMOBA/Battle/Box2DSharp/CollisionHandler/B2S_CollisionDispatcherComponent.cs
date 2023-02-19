using System;
using System.Collections.Generic;
namespace ET {

// 类似AIDispatcher，是全局的，整个进程只有一个，因为其本身就是一个无状态函数封装
    public class B2S_CollisionDispatcherComponent : Entity { // 碰撞检测，分发组件？
        public static B2S_CollisionDispatcherComponent Instance; // 单例模式
        public Dictionary<string, AB2S_CollisionHandler> B2SCollisionHandlers = new Dictionary<string, AB2S_CollisionHandler>();

// 处理碰撞开始，a碰到了b
        public void HandleCollisionStart(Unit a, Unit b) {
            if (B2SCollisionHandlers.TryGetValue(a.GetComponent<B2S_ColliderComponent>().CollisionHandlerName,
                                                 out var collisionHandler)) {
                collisionHandler.HandleCollisionStart(a, b);
            }
        }
        // 处理碰撞持续
        public void HandleCollisionSustain(Unit a, Unit b) {
            if (B2SCollisionHandlers.TryGetValue(a.GetComponent<B2S_ColliderComponent>().CollisionHandlerName,
                                                 out var collisionHandler)) {
                collisionHandler.HandleCollisionSustain(a, b);
            }
        }
        // 处理碰撞结束
        public void HandleCollsionEnd(Unit a, Unit b) {
            if (B2SCollisionHandlers.TryGetValue(a.GetComponent<B2S_ColliderComponent>().CollisionHandlerName,
                                                 out var collisionHandler)) {
                collisionHandler.HandleCollisionEnd(a, b);
            }
        }
    }
}