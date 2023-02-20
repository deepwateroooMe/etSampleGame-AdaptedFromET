using System;
using System.ComponentModel;
namespace ET {
    public interface ISupportInitialize {
        void BeginInit();
        void EndInit();
    }
    // 实现了 ISupportInitialize 之后：就可以自动完成很多工作 
    public abstract class Object: ISupportInitialize, IDisposable {
        public virtual void BeginInit() {
        }
        public virtual void EndInit() {
        }
        public virtual void Dispose() {
        }
        public override string ToString() {
            return MongoHelper.ToJson(this);
        }
    }
}