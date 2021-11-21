﻿using UnityEngine;

namespace ET
{
    [LSF_Tickable(EntityType = typeof(MoveComponent))]
    public class MoveComponentTicker : ALSF_TickHandler<MoveComponent>
    {
#if !SERVER
        public override bool OnLSF_CheckConsistency(MoveComponent entity, uint frame, ALSF_Cmd stateToCompare)
        {
            LSF_MoveCmd serverMoveState = stateToCompare as LSF_MoveCmd;

            // 由于我们客户端模拟服务端的帧数会比较激进的向上取整加上服务端的缓存帧机制，即服务端此时可能才跑在25.5帧，我们就当作它跑到26帧了，这就会有这样一种可能：我们客户端指令会超前/延后被服务端处理，也就会导致服务端指令延后/超前被客户端处理
            // 所以要往前往后对比一到二帧，消除这个激进的策略误差
            if (entity.HistroyMoveStates.TryGetValue(serverMoveState.Frame, out var histroyState))
            {
                bool result = serverMoveState.CheckConsistency(histroyState);

                if (!result)
                {
                    Log.Error(
                        $"---来自MoveComponent的不一致：服务端 {serverMoveState.Frame} X：{serverMoveState.PosX} Y: {serverMoveState.PosY} Z: {serverMoveState.PosZ}\n客户端：{frame} X：{histroyState.PosX} Y: {histroyState.PosY} Z: {histroyState.PosZ}");
                }
                else
                {
                    Log.Error(
                        $"√√√来自MoveComponent的一致：服务端 {serverMoveState.Frame} X：{serverMoveState.PosX} Y: {serverMoveState.PosY} Z: {serverMoveState.PosZ}\n客户端：{frame} X：{histroyState.PosX} Y: {histroyState.PosY} Z: {histroyState.PosZ}");
                }

                return result;
            }

            return true;
        }

        public override void OnLSF_PredictTick(MoveComponent entity, long deltaTime)
        {
            Unit unit = entity.GetParent<Unit>();

            LSF_Component lsfComponent = unit.BelongToRoom.GetComponent<LSF_Component>();

            LSF_MoveCmd lsfMoveCmd = ReferencePool.Acquire<LSF_MoveCmd>().Init(unit.Id) as LSF_MoveCmd;

            lsfMoveCmd.Speed = entity.Speed;
            lsfMoveCmd.PosX = unit.Position.x;
            lsfMoveCmd.PosY = unit.Position.y;
            lsfMoveCmd.PosZ = unit.Position.z;

            lsfMoveCmd.RotA = unit.Rotation.x;
            lsfMoveCmd.RotB = unit.Rotation.y;
            lsfMoveCmd.RotC = unit.Rotation.z;
            lsfMoveCmd.RotW = unit.Rotation.w;

            lsfMoveCmd.IsStopped = false;

            entity.HistroyMoveStates[lsfComponent.CurrentFrame] = lsfMoveCmd;
        }

        public override void OnLSF_RollBackTick(MoveComponent entity, uint frame, ALSF_Cmd stateToCompare)
        {
            LSF_MoveCmd lsfMoveCmd = stateToCompare as LSF_MoveCmd;
            Unit unit = entity.GetParent<Unit>();

            entity.Stop();
            Game.EventSystem.Publish(new EventType.MoveStop() {Unit = unit}).Coroutine();

            Vector3 pos = new Vector3(lsfMoveCmd.PosX, lsfMoveCmd.PosY, lsfMoveCmd.PosZ);
            Quaternion rotation = new Quaternion(lsfMoveCmd.RotA, lsfMoveCmd.RotB, lsfMoveCmd.RotC, lsfMoveCmd.RotW);
            unit.Position = pos;
            unit.Rotation = rotation;
        }

#endif

        public override void OnLSF_Tick(MoveComponent entity, long deltaTime)
        {
            Unit unit = entity.GetParent<Unit>();

            if (entity.ShouldMove)
            {
                entity.MoveForward(deltaTime, false);
#if !SERVER
                Log.Error(
                    $"////// MoveComponent Tick {unit.BelongToRoom.GetComponent<LSF_Component>().CurrentFrame} {unit.Position.ToString("0.0000")}");
#endif
            }

#if SERVER
            LSF_MoveCmd lsfMoveCmd = ReferencePool.Acquire<LSF_MoveCmd>().Init(unit.Id) as LSF_MoveCmd;

            lsfMoveCmd.Speed = entity.Speed;
            lsfMoveCmd.PosX = unit.Position.x;
            lsfMoveCmd.PosY = unit.Position.y;
            lsfMoveCmd.PosZ = unit.Position.z;

            lsfMoveCmd.RotA = unit.Rotation.x;
            lsfMoveCmd.RotB = unit.Rotation.y;
            lsfMoveCmd.RotC = unit.Rotation.z;
            lsfMoveCmd.RotW = unit.Rotation.w;

            lsfMoveCmd.IsStopped = false;

            unit.BelongToRoom.GetComponent<LSF_Component>().AddCmdToSendQueue(lsfMoveCmd);
            
            //Log.Info($"Frame: {unit.BelongToRoom.GetComponent<LSF_Component>().CurrentFrame} {entity.To.ToString()}");
#else
            //Log.Info($"Frame: {unit.BelongToRoom.GetComponent<LSF_Component>().CurrentFrame} {entity.To.ToString()}");


#endif
        }
    }
}