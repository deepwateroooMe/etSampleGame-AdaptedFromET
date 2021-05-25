//------------------------------------------------------------
// Author: 烟雨迷离半世殇
// Mail: 1778139321@qq.com
// Data: 2019年9月16日 21:12:21
//------------------------------------------------------------

using System;

namespace ETModel
{
    /// <summary>
    /// 瞬时伤害
    /// </summary>
    public class FlashDamageBuffSystem: ABuffSystemBase
    {
        public override void OnExecute()
        {
            FlashDamageBuffData flashDamageBuffData = this.GetSelfBuffData<FlashDamageBuffData>();
            DamageData damageData = ReferencePool.Acquire<DamageData>().InitData(flashDamageBuffData.BuffDamageTypes,
                BuffDataCalculateHelper.CalculateCurrentData(this, this.BuffData), this.TheUnitFrom, this.TheUnitBelongto,
                flashDamageBuffData.CustomData);

            damageData.DamageValue *= flashDamageBuffData.DamageFix;

            this.TheUnitFrom.GetComponent<CastDamageComponent>().BaptismDamageData(damageData);

            float finalDamage = this.TheUnitBelongto.GetComponent<ReceiveDamageComponent>().BaptismDamageData(damageData);

            if (finalDamage >= 0)
            {
                this.TheUnitBelongto.GetComponent<HeroDataComponent>().NumericComponent.ApplyChange(NumericType.Hp, -finalDamage);
                //抛出伤害事件
                Game.Scene.GetComponent<BattleEventSystem>().Run($"{EventIdType.ExcuteDamage}{this.TheUnitFrom.Id}", damageData);
                //抛出受伤事件
                Game.Scene.GetComponent<BattleEventSystem>().Run($"{EventIdType.TakeDamage}{this.GetBuffTarget().Id}", damageData);
            }

            //TODO 从当前战斗Entity获取BattleEventSystem来Run事件
            if (this.BuffData.EventIds != null)
            {
                foreach (var eventId in this.BuffData.EventIds)
                {
                    Game.Scene.GetComponent<BattleEventSystem>().Run($"{eventId}{this.TheUnitFrom.Id}", this);
                    //Log.Info($"抛出了{this.MSkillBuffDataBase.theEventID}{this.theUnitFrom.Id}");
                }
            }

            this.BuffState = BuffState.Finished;
            //Log.Info($"设置瞬时伤害Buff：{this.MSkillBuffDataBase.FlagId}状态为Finshed");
        }
    }
}