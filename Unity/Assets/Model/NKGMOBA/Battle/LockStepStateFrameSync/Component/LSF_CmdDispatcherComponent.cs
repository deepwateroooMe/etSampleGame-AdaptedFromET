using System;
using System.Collections.Generic;
namespace ET {

    // 专门处理状态帧消息
    public class LSF_CmdDispatcherComponent : Entity {
        public static LSF_CmdDispatcherComponent Instance; // 单例模式

        public readonly Dictionary<uint, List<ILockStepStateFrameSyncMessageHandler>> Handlers = new Dictionary<uint, List<ILockStepStateFrameSyncMessageHandler>>();
    }
}