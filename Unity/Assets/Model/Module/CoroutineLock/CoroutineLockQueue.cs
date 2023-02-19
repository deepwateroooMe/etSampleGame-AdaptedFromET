using System.Collections.Generic;
namespace ET {
    [ObjectSystem]
    public class CoroutineLockQueueAwakeSystem: AwakeSystem<CoroutineLockQueue> {
        public override void Awake(CoroutineLockQueue self) {
            self.queue.Clear();
        }
    }
    [ObjectSystem]
    public class CoroutineLockQueueDestroySystem: DestroySystem<CoroutineLockQueue> {
        public override void Destroy(CoroutineLockQueue self) {
            self.queue.Clear();
        }
    }
    public struct CoroutineLockInfo {
        public ETTask<CoroutineLock> Tcs;
        public int Time;
    }
    
    public class CoroutineLockQueue: Entity { // 协程锁队列：是用来作什么的？
        public Queue<CoroutineLockInfo> queue = new Queue<CoroutineLockInfo>(); // 队列：用来缓存吗？先进先出

        public void Add(ETTask<CoroutineLock> tcs, int time) { // 添加：就根据参加添加这个元素
            this.queue.Enqueue(new CoroutineLockInfo(){Tcs = tcs, Time = time});
        }
        public int Count {
            get {
                return this.queue.Count;
            }
        }
        public CoroutineLockInfo Dequeue() {
            return this.queue.Dequeue();
        }
    }
}