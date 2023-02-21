using System.Collections.Generic;
namespace ET { // 小游戏里，封装的简单对象池

    public class Pool<T> where T: class, new() {
        private readonly Queue<T> pool = new Queue<T>(); // 用的是，队列
        public T Fetch() {
            if (pool.Count == 0) {
                return new T();
            }
            return pool.Dequeue();
        }
        
        public void Recycle(T t) {
            pool.Enqueue(t);
        }
        public void Clear() {
            this.pool.Clear();
        }
    }
}