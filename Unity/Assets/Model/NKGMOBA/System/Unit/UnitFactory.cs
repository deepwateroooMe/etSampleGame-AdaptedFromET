﻿using ETModel.NKGMOBA.Battle.State;
using UnityEngine;

namespace ETModel
{
    public static class UnitFactory
    {
        public static Unit CreateHero(long id, int unitTypeId, RoleCamp roleCamp)
        {
            Client_UnitConfig clientUnitConfig = ConfigComponent.Instance.Get<Client_UnitConfig>(unitTypeId);
            string unitName = clientUnitConfig.UnitName;
            PrepareHeroRes(unitName);
            Unit unit = Game.Scene.GetComponent<GameObjectPool>().FetchEntityWithId(id, unitName);

            unit.AddComponent<DataModifierComponent>();
            unit.AddComponent<NumericComponent>();
            unit.AddComponent<HeroTransformComponent>();
            //增加子实体组件，用于管理子实体
            unit.AddComponent<ChildrenUnitComponent>();
            //增加栈式状态机，辅助动画切换
            unit.AddComponent<StackFsmComponent>();
            unit.AddComponent<AnimationComponent>();
            unit.AddComponent<MoveComponent>();
            unit.AddComponent<TurnComponent>();
            unit.AddComponent<UnitPathComponent>();
            unit.AddComponent<EffectComponent>();
            //增加Buff管理组件
            unit.AddComponent<BuffManagerComponent>();
            unit.AddComponent<SkillCanvasManagerComponent>();
            unit.AddComponent<B2S_RoleCastComponent, RoleCamp>(roleCamp);
            unit.AddComponent<CommonAttackComponent>();
            unit.AddComponent<NP_RuntimeTreeManager>();
            //英雄属性组件
            unit.AddComponent<UnitAttributesDataComponent, long>(clientUnitConfig.UnitAttributesDataId);

            unit.GameObject.GetComponent<MonoBridge>().BelongToUnit = unit;
            UnitComponent.Instance.Add(unit);
            
            //Log.Info("开始装载技能");
            NP_RuntimeTreeFactory.CreateSkillNpRuntimeTree(unit,
                ConfigComponent.Instance.Get<Client_SkillCanvasConfig>(clientUnitConfig.UnitPassiveSkillId).NPBehaveId,
                ConfigComponent.Instance.Get<Client_SkillCanvasConfig>(clientUnitConfig.UnitPassiveSkillId).BelongToSkillId).Start();
            NP_RuntimeTreeFactory.CreateSkillNpRuntimeTree(unit,
                ConfigComponent.Instance.Get<Client_SkillCanvasConfig>(clientUnitConfig.UnitQSkillId).NPBehaveId,
                ConfigComponent.Instance.Get<Client_SkillCanvasConfig>(clientUnitConfig.UnitQSkillId).BelongToSkillId).Start();
            NP_RuntimeTreeFactory.CreateSkillNpRuntimeTree(unit,
                ConfigComponent.Instance.Get<Client_SkillCanvasConfig>(clientUnitConfig.UnitWSkillId).NPBehaveId,
                ConfigComponent.Instance.Get<Client_SkillCanvasConfig>(clientUnitConfig.UnitWSkillId).BelongToSkillId).Start();
            NP_RuntimeTreeFactory.CreateSkillNpRuntimeTree(unit,
                ConfigComponent.Instance.Get<Client_SkillCanvasConfig>(clientUnitConfig.UnitESkillId).NPBehaveId,
                ConfigComponent.Instance.Get<Client_SkillCanvasConfig>(clientUnitConfig.UnitESkillId).BelongToSkillId).Start();
            //Log.Info("行为树创建完成");
            
            return unit;
        }

        /// <summary>
        /// 创建木桩
        /// </summary>
        /// <param name="selfId">自己的id</param>
        /// <param name="parentId">父实体id</param>
        /// <returns></returns>
        public static Unit CreateSpiling(long selfId, long parentId, RoleCamp roleCamp)
        {
            PrepareHeroRes("Darius");

            Unit unit = Game.Scene.GetComponent<GameObjectPool>().FetchEntityWithId(selfId, "Darius");
            //Log.Info($"此英雄的Model层ID为{unit.Id}");
            unit.AddComponent<DataModifierComponent>();
            unit.AddComponent<NumericComponent>();
            //增加子实体组件，用于管理子实体
            unit.AddComponent<ChildrenUnitComponent>();
            //增加栈式状态机，辅助动画切换
            unit.AddComponent<StackFsmComponent>();
            unit.AddComponent<MoveComponent>();
            unit.AddComponent<TurnComponent>();
            unit.AddComponent<UnitPathComponent>();
            unit.AddComponent<AnimationComponent>();

            unit.AddComponent<EffectComponent>();
            unit.AddComponent<B2S_RoleCastComponent, RoleCamp>(roleCamp);
            unit.AddComponent<HeroTransformComponent>();

            //增加Buff管理组件
            unit.AddComponent<BuffManagerComponent>();
            unit.GameObject.GetComponent<MonoBridge>().BelongToUnit = unit;
            UnitComponent.Instance.Get(parentId).GetComponent<ChildrenUnitComponent>().AddUnit(unit);
            return unit;
        }

        /// <summary>
        /// 准备英雄资源
        /// </summary>
        /// <param name="heroType"></param>
        private static void PrepareHeroRes(string heroType)
        {
            ResourcesComponent resourcesComponent = Game.Scene.GetComponent<ResourcesComponent>();
            GameObject bundleGameObject = resourcesComponent.LoadAsset<GameObject>(ABPathUtilities.GetUnitPath("Unit"));
            GameObject prefab = bundleGameObject.GetTargetObjectFromRC<GameObject>(heroType);
            Game.Scene.GetComponent<GameObjectPool>().Add(heroType, prefab);
        }

        /// <summary>
        /// 用于NPBehave测试
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static void NPBehaveTestCreate()
        {
            Unit unit = ComponentFactory.Create<Unit>();
            unit.AddComponent<NP_RuntimeTreeManager>();
            UnitComponent.Instance.Add(unit);
            //NP_RuntimeTreeFactory.CreateNpRuntimeTree(unit, NP_Client_TreeIds.Darius_Q_Client).Start();
        }
    }
}