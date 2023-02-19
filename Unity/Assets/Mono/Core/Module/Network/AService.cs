using System;
using System.IO;
using System.Net;
namespace ET {
    
// 所有网络调用服务的抽象基类：
    public abstract class AService: IDisposable {
        public ServiceType ServiceType { get; protected set; } // 内网＋外网消息 
        public ThreadSynchronizationContext ThreadSynchronizationContext;
        
        private long connectIdGenerater = int.MaxValue; // 最大值 
        public long CreateConnectChannelId(uint localConn) {
            return (--this.connectIdGenerater << 32) | localConn; // localConn放在低32bit
        }
        public uint CreateRandomLocalConn() { // 不知道是干什么用的
            return (1u << 30) | RandomHelper.RandUInt32();
        }
        private long acceptIdGenerater = 1; // 最小值：从两端，为的是尽可能地不交叉
        public long CreateAcceptChannelId(uint localConn) {
            return (++this.acceptIdGenerater << 32) | localConn; // localConn放在低32bit
        }

        public abstract void Update();
        public abstract void Remove(long id);
        
        public abstract bool IsDispose();
        protected abstract void Get(long id, IPEndPoint address);
        public abstract void Dispose();
        protected abstract void Send(long channelId, long actorId, MemoryStream stream);
// 几个回调：
        protected void OnAccept(long channelId, IPEndPoint ipEndPoint) {
            this.AcceptCallback.Invoke(channelId, ipEndPoint);
        }
        public void OnRead(long channelId, MemoryStream memoryStream) {
            this.ReadCallback.Invoke(channelId, memoryStream);
        }
        public void OnError(long channelId, int e) {
            this.Remove(channelId);
            this.ErrorCallback?.Invoke(channelId, e);
        }
        public Action<long, IPEndPoint> AcceptCallback;
        public Action<long, int> ErrorCallback;
        public Action<long, MemoryStream> ReadCallback;

        public void Destroy() {
            this.Dispose();
        }
        public void RemoveChannel(long channelId) {
            this.Remove(channelId);
        }
        public void SendStream(long channelId, long actorId, MemoryStream stream) {
            this.Send(channelId, actorId, stream);
        }
        public void GetOrCreate(long id, IPEndPoint address) {
            this.Get(id, address);
        }
    }
}