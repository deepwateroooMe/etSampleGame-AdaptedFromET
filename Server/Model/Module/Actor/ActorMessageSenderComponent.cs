using System.Collections.Generic;
namespace ET {
    public class ActorMessageSenderComponent: Entity {

        public static long TIMEOUT_TIME = 40 * 1000; // 设置；40 秒超时
        public static ActorMessageSenderComponent Instance { get; set; }
        public int RpcId;
        public readonly SortedDictionary<int, ActorMessageSender> requestCallback = new SortedDictionary<int, ActorMessageSender>(); // 是按，超时时间升序排列的吗？
        public long TimeoutCheckTimer;
        public List<int> TimeoutActorMessageSenders = new List<int>();
    }
}