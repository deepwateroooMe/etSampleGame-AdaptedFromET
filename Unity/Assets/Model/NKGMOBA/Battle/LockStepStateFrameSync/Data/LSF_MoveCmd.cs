﻿using ProtoBuf;
using UnityEngine;

namespace ET
{
    [ProtoContract]
    public class LSF_MoveCmd : ALSF_Cmd
    {
        private const uint c_LSF_CmdType = LSF_CmdType.Move;

        [ProtoMember(2)] public float PosX;
        [ProtoMember(3)] public float PosY;
        [ProtoMember(4)] public float PosZ;

        [ProtoMember(5)] public float RotA;
        [ProtoMember(6)] public float RotB;
        [ProtoMember(7)] public float RotC;
        [ProtoMember(8)] public float RotW;

        [ProtoMember(9)] public float Speed;
        [ProtoMember(10)] public bool IsStopped;
        
        public override ALSF_Cmd Init(uint frame)
        {
            this.Frame = frame;
            this.LockStepStateFrameSyncDataType = c_LSF_CmdType;

            return this;
        }

        public override void Clear()
        {
            base.Clear();
            PosX = 0;
            PosY = 0;
            PosZ = 0;
            RotA = 0;
            RotB = 0;
            RotC = 0;
            RotW = 0;
            Speed = 0;
            IsStopped = false;
            Frame = 0;
        }
    }
}