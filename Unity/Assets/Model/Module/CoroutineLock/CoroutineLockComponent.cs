using System.Collections.Generic;
namespace ET {
    [ObjectSystem]
    public class CoroutineLockComponentAwakeSystem: AwakeSystem<CoroutineLockComponent> {
        public override void Awake(CoroutineLockComponent self) {
            CoroutineLockComponent.Instance = self; // 创建实例化时，单例赋值
            for (int i = 0; i < self.list.Capacity; ++i) { // 数据结构相对复杂一点儿，非值类型，实例化时，初始化链表里的8 个有序字典元素
                self.list.Add(self.AddChildWithId<CoroutineLockQueueType>(++self.idGenerator));
            }
        }
    }
    [ObjectSystem]
    public class CoroutineLockComponentDestroySystem: DestroySystem<CoroutineLockComponent> {
        public override void Destroy(CoroutineLockComponent self) {
            self.list.Clear(); // 感觉这里粗暴了点儿：一般情况下不是说，因为链表里的每个元素都是索引 reference 类型，需要考虑到他们的如对象池般的回收再利用吗？若是不用回收，这么着也能清空字典元素里的所有键值内容？！应该是的
            self.nextFrameRun.Clear();
            self.timers.Clear();
            self.timeOutIds.Clear();
            self.timerOutTimer.Clear();
            self.idGenerator = 0;
            self.minTime = 0;
        }
    }
    
// 这个重要类：感觉绝大部分看懂了，一小部分细节还没有看懂    
    public class CoroutineLockComponentUpdateSystem: UpdateSystem<CoroutineLockComponent> {
        public override void Update(CoroutineLockComponent self) {
            // 检测超时的CoroutineLock
            TimeoutCheck(self); // <<<<<<<<<< 定义在下面
            int count = self.nextFrameRun.Count;
            // 注意这里不能将this.nextFrameRun.Count 放到for循环中，因为循环过程中会有对象继续加入队列
            for (int i = 0; i < count; ++i) {
                (CoroutineLockType coroutineLockType, long key) = self.nextFrameRun.Dequeue();
                self.Notify(coroutineLockType, key, 0);
            }
        }
        public void TimeoutCheck(CoroutineLockComponent self) {
            // 超时的锁
            if (self.timers.Count == 0) { // 没有待处理的任何任务
                return;
            }
            long timeNow = TimeHelper.ClientFrameTime();
            if (timeNow < self.minTime) { // 所有协程任务，都仍还处于，必须等待状态。需要再等
                return;
            } // 下面：就一定是有待处理的协程任务了
            foreach (KeyValuePair<long, List<CoroutineLockTimer>> kv in self.timers) {
                long k = kv.Key;
                if (k > timeNow) { // 仍必须等
                    self.minTime = k;
                    break; // 使用键有序（升序）字典：好处就是这里，直接退出循环
                }
                self.timeOutIds.Enqueue(k); // 这里把超时了的键，给保存起来：是因为它只遍历头遍历出待处理 Ids, 并不曾真正做任何实事
            }
            self.timerOutTimer.Clear(); // 清空待用缓存队列 
            while (self.timeOutIds.Count > 0) {
                long time = self.timeOutIds.Dequeue();
                foreach (CoroutineLockTimer coroutineLockTimer in self.timers[time]) { // 遍历值：是链表结构
                    self.timerOutTimer.Enqueue(coroutineLockTimer);
                }
                self.timers.Remove(time);
            }
            while (self.timerOutTimer.Count > 0) {
                CoroutineLockTimer coroutineLockTimer = self.timerOutTimer.Dequeue();
                if (coroutineLockTimer.CoroutineLockInstanceId != coroutineLockTimer.CoroutineLock.InstanceId) { // 这里的原理，可能发生这种情况的上下文，没想明白 
                    continue;
                }
                CoroutineLock coroutineLock = coroutineLockTimer.CoroutineLock;
                // 超时直接调用下一个锁【超时了：就需要执行协程的下一桢，下一个步骤】
                self.NextFrameRun(coroutineLock.coroutineLockType, coroutineLock.key);
                coroutineLock.coroutineLockType = CoroutineLockType.None; // 上面调用了下一个, dispose不再调用
            }
        }
    }
    public static class CoroutineLockComponentSystem {
        public static void NextFrameRun(this CoroutineLockComponent self, CoroutineLockType coroutineLockType, long key) {
            self.nextFrameRun.Enqueue((coroutineLockType, key)); // 仍然是，把下一桢必须得执行的（结构体）任务，缓存到缓存队列里，等到什么时候执行？直正回调FrameFinish? 之类的调用的时候？
        }
        // 超时字典里，添加新超时锁： tillTime, 是指超时的时间点，不是还要等待的时长。按超时时间点升序排列
        public static void AddTimer(this CoroutineLockComponent self, long tillTime, CoroutineLock coroutineLock) {
            self.timers.Add(tillTime, new CoroutineLockTimer(coroutineLock));
            if (tillTime < self.minTime) { // 同步更新：最短等待时间 
                self.minTime = tillTime;
            }
        }
        public static async ETTask<CoroutineLock> Wait(this CoroutineLockComponent self, CoroutineLockType coroutineLockType, long key, int time = 60000) {
            CoroutineLockQueueType coroutineLockQueueType = self.list[(int) coroutineLockType]; // 字典：值为，锁队列 Queue<CoroutineLockInfo>
            if (!coroutineLockQueueType.TryGetValue(key, out CoroutineLockQueue queue)) { // <<<<<<<<<< queue
                // 创建：从加工厂（对象池）创建一个字典里不存在元素 key 所对应的值， CoroutineLockQueue
                coroutineLockQueueType.Add(key, self.AddChildWithId<CoroutineLockQueue>(++self.idGenerator, true)); // 只在大类型链表－值，字典结构里，添加了这个 key 的存在
                // 上面的：只添加了【 key, 新生成的无名 Queue<CoroutineLockInfo>】键值对。常识一般添加协程锁，锁的时间 time>0, 那么真正锁的添加进队列是在上面 line 78 才真正添加锁进队列 
                return self.CreateCoroutineLock(coroutineLockType, key, time, 1); // 仍然需要创建一把超时协程锁
            }
            ETTask<CoroutineLock> tcs = ETTask<CoroutineLock>.Create(true); // 创建：实则是，从对象池中去取一个，返回类型为 CoroutineLock 类型的 ETTask 任务
            queue.Add(tcs, time); // 更新键升序字典里，相应键 key 的值（多添加一把超时锁）
            return await tcs;
        }
        // 创建一把超时协程锁： 
        public static CoroutineLock CreateCoroutineLock(this CoroutineLockComponent self, CoroutineLockType coroutineLockType, long key, int time, int count) {
            CoroutineLock coroutineLock = self.AddChildWithId<CoroutineLock, CoroutineLockType, long, int>(++self.idGenerator, coroutineLockType, key, count, true); // 创建锁
            if (time > 0) { // 添加超时机制（默认一般协程锁超时时间 time 》 0, 否则上面 line 88 会添加空值，队列中的值为空） 
                self.AddTimer(TimeHelper.ClientFrameTime() + time, coroutineLock);
            }
            return coroutineLock;
        }
        
// 这个方法里；一部分没有看懂
        public static void Notify(this CoroutineLockComponent self, CoroutineLockType coroutineLockType, long key, int count) {
            CoroutineLockQueueType coroutineLockQueueType = self.list[(int) coroutineLockType]; // 字典：值为，锁队列 Queue<CoroutineLock>
            if (!coroutineLockQueueType.TryGetValue(key, out CoroutineLockQueue queue)) {
                return;
            }
            if (queue.Count == 0) { // 接上面 line 99, 如果为空，直接移除
                coroutineLockQueueType.Remove(key);
                return;
            }
#if SERVER
            const int frameCoroutineCount = 5;
#else
            const int frameCoroutineCount = 10;
#endif
            if (count > frameCoroutineCount) { // 不明白：这里为什么要这么设置 ?
                self.NextFrameRun(coroutineLockType, key); // 把一个队列 Queue<CoroutineLockInfo> 里的超时锁全部执行锁，全部缓存待执行？
                return;
            }
            CoroutineLockInfo coroutineLockInfo = queue.Dequeue(); // 这里有一点儿不有想明白：这个系统队列里的一队列，怎么就能保证第一个取出来的超时锁就是时间最早的？这个时间关系是如何保证的？
            coroutineLockInfo.Tcs.SetResult(self.CreateCoroutineLock(coroutineLockType, key, coroutineLockInfo.Time, count));
        }
    }
    public class CoroutineLockComponent: Entity { // （系统的）协程锁组件：是对协程锁的综合管理
        public static CoroutineLockComponent Instance; // 单例模式
        public List<CoroutineLockQueueType> list = new List<CoroutineLockQueueType>((int) CoroutineLockType.Max); // 链表中元素：每个元素是字典。共最多8 个键
        public Queue<(CoroutineLockType, long)> nextFrameRun = new Queue<(CoroutineLockType, long)>();
        public MultiMap<long, CoroutineLockTimer> timers = new MultiMap<long, CoroutineLockTimer>(); // 按超时时间，键升序管理的有序字典：因为每一桢，按超时时间（键）升序遍历待处理协程任务
        public Queue<long> timeOutIds = new Queue<long>(); // 上面的，有序字典，里的键，超时了的键（时间）缓存入，这个队列
        public Queue<CoroutineLockTimer> timerOutTimer = new Queue<CoroutineLockTimer>();
        public long idGenerator;
        public long minTime; // 维护一个方便自己的闹钟：所有待处理协程里，最短必须得等多久，才有可处理的任务？
    }
}