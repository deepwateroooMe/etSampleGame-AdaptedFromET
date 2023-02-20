using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
namespace ET {
    public struct ProcessActorId {
        public int Process;
        public long ActorId;

        public ProcessActorId(long actorId) {
            InstanceIdStruct instanceIdStruct = new InstanceIdStruct(actorId);
            this.Process = instanceIdStruct.Process;
            instanceIdStruct.Process = GlobalDefine.Options.Process;
            this.ActorId = instanceIdStruct.ToLong();
        }
    }

    public class NetInnerComponent: Entity {
        public static NetInnerComponent Instance;
        public AService Service;
        public IMessageDispatcher MessageDispatcher { get; set; }
    }
}