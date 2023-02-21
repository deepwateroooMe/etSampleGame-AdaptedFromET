using System;
namespace ET {
    [ObjectSystem]
    public class CoroutineLockAwakeSystem: AwakeSystem<CoroutineLock, CoroutineLockType, long, int> {
        public override void Awake(CoroutineLock self, CoroutineLockType type, long k, int count) { // 创建时，所有三个参数全传进来
            self.coroutineLockType = type;
            self.key = k;
            self.count = count;
        }
    }
    [ObjectSystem]
    public class CoroutineLockDestroySystem: DestroySystem<CoroutineLock> {
        public override void Destroy(CoroutineLock self) {
            if (self.coroutineLockType != CoroutineLockType.None) { // 当锁还没有释放，要调用通知解锁。 count 是什么意思呢？
                CoroutineLockComponent.Instance.Notify(self.coroutineLockType, self.key, self.count + 1);
            } else {
                // CoroutineLockType.None说明协程锁超时了
                Log.Error($"coroutine lock timeout: {self.coroutineLockType} {self.key} {self.count}");
            }
            self.coroutineLockType = CoroutineLockType.None;
            self.key = 0;
            self.count = 0;
        }
    }
    
    public class CoroutineLock: Entity {
        public CoroutineLockType coroutineLockType; // 分类型：主要是，6 种不同使用上下文类型的锁
        public long key;
        public int count;
    }
}