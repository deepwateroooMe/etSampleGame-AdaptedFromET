//------------------------------------------------------------
// Author: 烟雨迷离半世殇
// Mail: 1778139321@qq.com
// Data: 2019年9月16日 22:28:26
//------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace ETModel
{
    /// <summary>
    /// Buff工厂
    /// </summary>
    public static class BuffFactory
    {
        /// <summary>
        /// 记录所有BuffSystem类型，用于运行时创建对应的BuffSystem
        /// </summary>
        public static Dictionary<BuffSystemType, Type> AllBuffSystemTypes = new Dictionary<BuffSystemType, Type>()
        {
            //TODO 如果要加新的Buff逻辑类型，需要在这里拓展，本人架构能力的确有限。。。
            { BuffSystemType.ChangePropertyBuffSystem, typeof (ChangePropertyBuffSystem) },
            { BuffSystemType.ListenBuffCallBackBuffSystem, typeof (ListenBuffCallBackBuffSystem) },
            { BuffSystemType.BindStateBuffSystem, typeof (BindStateBuffSystem) },
            { BuffSystemType.PlayEffectBuffSystem, typeof (PlayEffectBuffSystem) },
            { BuffSystemType.ReplaceAnimBuffSystem, typeof (ReplaceAnimBuffSystem) },
            { BuffSystemType.RefreshTargetBuffTimeBuffSystem, typeof (RefreshTargetBuffTimeBuffSystem) },
            { BuffSystemType.ChangeRenderAssetBuffSystem, typeof (ChangeRenderAssetBuffSystem) },
            { BuffSystemType.ChangeMaterialBuffSystem, typeof (ChangeMaterialBuffSystem) },
        };

        /// <summary>
        /// 取得Buff,Buff流程是Acquire->OnInit(CalculateTimerAndOverlay)->AddTemp->经过筛选->AddReal
        /// </summary>
        /// <param name="dataId">Buff数据归属的数据块Id</param>
        /// <param name="buffNodeId">Buff节点的Id</param>
        /// <param name="theUnitFrom">Buff来源者</param>
        /// <param name="theUnitBelongTo">Buff寄生者</param>
        /// <returns></returns>
        public static IBuffSystem AcquireBuff(long dataId, long buffNodeId, Unit theUnitFrom, Unit theUnitBelongTo,
        NP_RuntimeTree theSkillCanvasBelongTo)
        {
            return AcquireBuff(
                (Game.Scene.GetComponent<NP_TreeDataRepository>().GetNP_TreeData(dataId).BuffNodeDataDic[buffNodeId] as NormalBuffNodeData).BuffData,
                theUnitFrom, theUnitBelongTo, theSkillCanvasBelongTo);
        }

        /// <summary>
        /// 取得Buff,Buff流程是Acquire->OnInit(CalculateTimerAndOverlay)->AddTemp->经过筛选->AddReal
        /// </summary>
        /// <param name="npDataSupportor">Buff数据归属的数据块</param>
        /// <param name="buffNodeId">Buff节点的Id</param>
        /// <param name="theUnitFrom">Buff来源者</param>
        /// <param name="theUnitBelongTo">Buff寄生者</param>
        /// <returns></returns>
        public static IBuffSystem AcquireBuff(NP_DataSupportor npDataSupportor, long buffNodeId, Unit theUnitFrom, Unit theUnitBelongTo,
        NP_RuntimeTree theSkillCanvasBelongTo)
        {
            return AcquireBuff((npDataSupportor.BuffNodeDataDic[buffNodeId] as NormalBuffNodeData).BuffData, theUnitFrom, theUnitBelongTo,
                theSkillCanvasBelongTo);
        }

        /// <summary>
        /// 取得Buff,Buff流程是Acquire->OnInit(CalculateTimerAndOverlay)->AddTemp->经过筛选->AddReal
        /// </summary>
        /// <param name="buffDataBase">Buff数据</param>
        /// <param name="theUnitFrom">Buff来源者</param>
        /// <param name="theUnitBelongTo">Buff寄生者</param>
        /// <returns></returns>
        public static IBuffSystem AcquireBuff(BuffDataBase buffDataBase, Unit theUnitFrom, Unit theUnitBelongTo,
        NP_RuntimeTree theSkillCanvasBelongTo)
        {
            IBuffSystem resultBuff = ReferencePool.Acquire(AllBuffSystemTypes[buffDataBase.BelongBuffSystemType]) as IBuffSystem;
            resultBuff.BelongtoRuntimeTree = theSkillCanvasBelongTo;
            resultBuff.Init(buffDataBase, theUnitFrom, theUnitBelongTo);
            return resultBuff;
        }

        /// <summary>
        /// 回收一个Buff
        /// </summary>
        /// <param name="aBuffSystemBase"></param>
        public static void RecycleBuff<T>(ABuffSystemBase<T> aBuffSystemBase) where T : BuffDataBase
        {
            ReferencePool.Release(aBuffSystemBase);
        }
    }
}