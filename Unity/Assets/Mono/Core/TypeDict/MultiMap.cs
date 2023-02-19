using System.Collections.Generic;
namespace ET {
    // 自定义类：键有序（默认升序）的字典，字典的值为链表。自定义后，框架里使用时，写法可以大量简化 
    public class MultiMap<T, K>: SortedDictionary<T, List<K>> {
        private readonly List<K> Empty = new List<K>();
        public void Add(T t, K k) {
            List<K> list;
            this.TryGetValue(t, out list);
            if (list == null) {
                list = new List<K>();
                this.Add(t, list);
            }
            list.Add(k);
        }
        public bool Remove(T t, K k) {
            List<K> list;
            this.TryGetValue(t, out list);
            if (list == null) {
                return false;
            }
            if (!list.Remove(k)) {
                return false;
            }
            if (list.Count == 0) {
                this.Remove(t);
            }
            return true;
        }
        // 不返回内部的list,copy一份出来
        public K[] GetAll(T t) {
            List<K> list;
            this.TryGetValue(t, out list);
            if (list == null) {
                return new K[0];
            }
            return list.ToArray();
        }
        // 返回内部的list
        public new List<K> this[T t] {
            get {
                this.TryGetValue(t, out List<K> list);
                return list ?? Empty;
            }
        }
        public K GetOne(T t) {
            List<K> list;
            this.TryGetValue(t, out list);
            if (list != null && list.Count > 0) {
                return list[0];
            }
            return default;
        }
        public bool Contains(T t, K k) {
            List<K> list;
            this.TryGetValue(t, out list);
            if (list == null) {
                return false;
            }
            return list.Contains(k);
        }
    }
}