mespace ET {
    public static class MessageHelper {
        // 将消息广播给房间所有玩家
        public static void BroadcastToRoom(Room room, IActorMessage message) {
            var units = room.GetComponent<UnitComponent>().GetAll();
            if (units == null) return;
            foreach (Unit u in units) {
                UnitGateComponent unitGateComponent = u.GetComponent<UnitGateComponent>();
                if (unitGateComponent != null) {
                    SendActor(unitGateComponent.GateSessionActorId, message);
                }
            }
        }
        // 发送协议给ActorLocation
        public static void SendToLocationActor(long id, IActorLocationMessage message) {
            ActorLocationSenderComponent.Instance.Send(id, message);
        }
        
        // 发送协议给Actor
        public static void SendActor(long actorId, IActorMessage message) {
            ActorMessageSenderComponent.Instance.Send(actorId, message);
        }
        // 发送RPC协议给Actor
        public static async ETTask<IActorResponse> CallActor(long actorId, IActorRequest message) {
            return await ActorMessageSenderComponent.Instance.Call(actorId, message);
        }
        // 发送RPC协议给ActorLocation
        public static async ETTask<IActorResponse> CallLocationActor(long id, IActorLocationRequest message) {
            return await ActorLocationSenderComponent.Instance.Call(id, message);
        }
    }
}