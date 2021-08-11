//------------------------------------------------------------
// Author: 烟雨迷离半世殇
// Mail: 1778139321@qq.com
// Data: 2019年10月2日 17:03:26
//------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

namespace ETModel
{
    public static class BuffTimerAndOverlayHelper
    {
        /// <summary>
        /// 为Buff计算时间和层数
        /// </summary>
        /// <param name="buffSystemBase">Buff逻辑类</param>
        /// <typeparam name="T"></typeparam>
        public static void CalculateTimerAndOverlay<T>(ABuffSystemBase<T> buffSystemBase) where T : BuffDataBase
        {
            BuffManagerComponent buffManagerComponent = buffSystemBase.GetBuffTarget().GetComponent<BuffManagerComponent>();

            //先尝试从Buff链表取得Buff
            IBuffSystem targetBuffSystemBase = buffManagerComponent.GetBuffById(buffSystemBase.BuffData.BuffId);

            if (targetBuffSystemBase != null)
            {
                CalculateTimerAndOverlayHelper(targetBuffSystemBase as ABuffSystemBase<T>);
                //Log.Info($"本次续命BuffID为{buffDataBase.FlagId}，当前层数{temp.CurrentOverlay}，最高层为{temp.MSkillBuffDataBase.MaxOverlay}");
                buffSystemBase.CurrentOverlay = targetBuffSystemBase.CurrentOverlay;
                //刷新当前已有的Buff
                targetBuffSystemBase.Refresh();
            }
            else
            {
                CalculateTimerAndOverlayHelper(buffSystemBase);

                //Log.Info($"本次新加BuffID为{buffDataBase.FlagId}");
                buffManagerComponent.AddBuff(buffSystemBase);
            }
        }

        /// <summary>
        /// 计算刷新的持续时间和层数
        /// </summary>
        private static void CalculateTimerAndOverlayHelper<T>(ABuffSystemBase<T> targetBuffSystemBase) where T : BuffDataBase
        {
            //可以叠加，并且当前层数加上要添加Buff的目标层数未达到最高层
            if (targetBuffSystemBase.BuffData.CanOverlay)
            {
                if (targetBuffSystemBase.CurrentOverlay + targetBuffSystemBase.BuffData.TargetOverlay <=
                    targetBuffSystemBase.BuffData.MaxOverlay)
                {
                    targetBuffSystemBase.CurrentOverlay += targetBuffSystemBase.BuffData.TargetOverlay;
                }
                else
                {
                    targetBuffSystemBase.CurrentOverlay = targetBuffSystemBase.BuffData.MaxOverlay;
                }
            }
            else
            {
                targetBuffSystemBase.CurrentOverlay = 1;
            }

            //如果是有限时长的 TODO:这里考虑处理持续时间和Buff层数挂钩的情况（比如磕了5瓶药，就是5*单瓶药的持续时间）
            if (targetBuffSystemBase.BuffData.SustainTime + 1 > 0)
            {
                //Log.Info($"原本结束时间：{temp.MaxLimitTime},续命之后的结束时间{TimeHelper.Now() + buffDataBase.SustainTime}");
                targetBuffSystemBase.MaxLimitTime = TimeHelper.Now() + targetBuffSystemBase.BuffData.SustainTime;
            }
        }
    }
}