using System;
using System.Collections.Generic;
namespace ET {
    public class RoomAwakeSystem : AwakeSystem<Room> {
        public override void Awake(Room self) { // 呵呵：如果这个是空的，为什么一定需要注册？ Awake() 是不注册不可以的吗？
        }
    }
    public class  RoomDestorySystem : DestroySystem<Room> {
        public override void Destroy(Room self) {
            self.enterNum = 0;
            self.RoomHolder = null;
            self.startGameNum = 0;
            self.ContainsPlayers.Clear();
            self.PlayersCamp.Clear();
        }
    }
    // 代表一个房间
    public class Room : Entity {
        // 房主
        public Player RoomHolder;
        public string RoomName;
        public int enterNum;
        public int startGameNum;
        // 这个房间当前包含的玩家，包括房主
        public Dictionary<long, Player> ContainsPlayers = new Dictionary<long, Player>();
        // 房间玩家对应的位置，也就是阵营
        public Dictionary<int, long> PlayersCamp = new Dictionary<int, long>();
    }
}