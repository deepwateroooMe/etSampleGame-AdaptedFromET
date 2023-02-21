using System.Collections.Generic;
namespace ET {
    // 可以方便的获取对应的StartSceneConfig
    public static class AddressHelper {
        // 随机分配一个网关服
        // <param name="zone"></param>
        public static StartSceneConfig GetGate(int zone) {
            List<StartSceneConfig> zoneGates = StartSceneConfigCategory.Instance.Gates[zone];
            
            int n = RandomHelper.RandomNumber(0, zoneGates.Count);
            return zoneGates[n];
        }
        
        // 随机分配一个大厅服
        // <param name="zone"></param>
        public static StartSceneConfig GetLobby(int zone) {
            List<StartSceneConfig> zoneGates = StartSceneConfigCategory.Instance.Lobbys[zone];
            
            int n = RandomHelper.RandomNumber(0, zoneGates.Count);
            return zoneGates[n];
        }
    }
}
