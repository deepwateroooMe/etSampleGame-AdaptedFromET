using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
namespace ET {
    public class NetThreadComponent: Entity {
        public static NetThreadComponent Instance; // 单例模式

        public const int checkInteral = 2000; // 2 秒
        public const int recvMaxIdleTime = 60000; // 1 分钟
        public const int sendMaxIdleTime = 60000; // 1 分钟
        public ThreadSynchronizationContext ThreadSynchronizationContext; // 同步上下文 
        public HashSet<AService> Services = new HashSet<AService>();  // 服务集
    }
}