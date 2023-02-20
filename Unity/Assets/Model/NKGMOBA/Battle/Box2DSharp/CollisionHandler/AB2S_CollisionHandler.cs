namespace ET {

    public class B2S_CollisionHandlerAttribute : BaseAttribute {}

    [B2S_CollisionHandler]
    public abstract class AB2S_CollisionHandler {
        // a是碰撞者自身，b是碰撞到的目标
        public abstract void HandleCollisionStart(Unit a, Unit b);
        // a是碰撞者自身，b是碰撞到的目标
        public abstract void HandleCollisionSustain(Unit a, Unit b);
        // a是碰撞者自身，b是碰撞到的目标
        public abstract void HandleCollisionEnd(Unit a, Unit b);
    }
}