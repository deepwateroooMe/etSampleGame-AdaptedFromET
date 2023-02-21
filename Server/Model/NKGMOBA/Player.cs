using System;
namespace ET {
    public class PlayerSystem : AwakeSystem<Player, string> {
        public override void Awake(Player self, string a) {
            self.Awake(a);
        }
    }
    public sealed class Player : Entity {
        public string Account { get; private set; }
        public long UnitId { get; set; }
        public long GateSessionId { get; set; }
        public Session GateSession { get; set; }
        public Session LobbySession { get; set; }
        // 所归属的RoomId
        public long RoomId { get; set; }
        // 所归属的阵营
        public Int32 camp { get; set; }
        public void Awake(string account) {
            this.Account = account;
        }
    }
}