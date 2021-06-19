//------------------------------------------------------------
// Author: 烟雨迷离半世殇
// Mail: 1778139321@qq.com
// Data: 2019/8/14 16:22:55
// Description: 此代码switch case与System部分由工具生成，请勿进行增减操作
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using ETModel;
using ETModel.BBValues;
using UnityEngine;

namespace ETHotfix
{
    [Event(EventIdType_Collision.B2S_Darius_Q_CRS)]
    public class AddB2S_Darius_Q_CRSSystem: AEvent<Entity>
    {
        public override void Run(Entity a)
        {
            a.AddComponent<B2S_Darius_Q_CRS>();
            //Log.Info($"诺手Q碰撞体创建完成，帧步进为{BenchmarkHelper.CurrentFrameCount}");
        }
    }

    [ObjectSystem]
    public class B2S_Darius_Q_CRSAwakeSystem: AwakeSystem<B2S_Darius_Q_CRS>
    {
        public override void Awake(B2S_Darius_Q_CRS self)
        {
            self.Entity.GetComponent<B2S_CollisionResponseComponent>().OnCollideStartAction += self.OnCollideStart;
            self.Entity.GetComponent<B2S_CollisionResponseComponent>().OnCollideSustainAction += self.OnCollideSustain;
            self.Entity.GetComponent<B2S_CollisionResponseComponent>().OnCollideFinishAction += self.OnCollideFinish;
        }
    }

    public class B2S_Darius_Q_CRS: Component
    {
        /// <summary>
        /// 当发生碰撞时
        /// </summary>
        /// <param name="b2SCollider">碰撞到的对象</param>
        public void OnCollideStart(Entity b2SCollider)
        {
            //技能碰撞体自身Unit的B2S_ColliderComponent
            B2S_ColliderComponent selfColliderComponent = Entity.GetComponent<B2S_ColliderComponent>();
            //碰撞到的Unit的B2S_ColliderComponent
            B2S_ColliderComponent targetColliderComponent = b2SCollider.GetComponent<B2S_ColliderComponent>();

            //自身Collider Unit所归属的Unit
            Unit selfBelongToUnit = selfColliderComponent.BelongToUnit;
            //碰撞到的Collider Unit所归属的Unit
            Unit collisionBelongToUnit = targetColliderComponent.BelongToUnit;

            if (selfBelongToUnit.GetComponent<B2S_RoleCastComponent>()
                        .GetRoleCastToTarget(collisionBelongToUnit) !=
                RoleCast.Adverse) return;

            //获取目标SkillCanvas
            List<NP_RuntimeTree> targetSkillCanvas = this.Entity.GetComponent<SkillCanvasManagerComponent>()
                    .GetSkillCanvas(Game.Scene.GetComponent<ConfigComponent>().Get<Server_SkillCanvasConfig>(10003).BelongToSkillId);

            //敌方英雄
            if (Vector3.Distance(selfBelongToUnit.Position, collisionBelongToUnit.Position) >= 2.3f)
            {
                //Log.Info("Q技能打到了诺克，外圈，开始添加Buff");

                foreach (var skillCanvas in targetSkillCanvas)
                {
                    skillCanvas.GetBlackboard().Set("Darius_QOutIsHitUnit", true);
                    skillCanvas.GetBlackboard().Get<List<long>>("Darius_QOutHitUnitIds")?.Add(collisionBelongToUnit.Id);
                }
            }
            else
            {
                //Log.Info("Q技能打到了诺克，内圈，开始添加Buff");

                foreach (var skillCanvas in targetSkillCanvas)
                {
                    skillCanvas.GetBlackboard().Set("Darius_QInnerIsHitUnit", true);
                    skillCanvas.GetBlackboard().Get<List<long>>("Darius_QInnerHitUnitIds")?.Add(collisionBelongToUnit.Id);
                }
            }
        }

        public void OnCollideSustain(Entity b2SCollider)
        {
            //Log.Info("持续碰撞了");
        }

        public void OnCollideFinish(Entity b2SCollider)
        {
            //Log.Info("不再碰撞了");
        }
    }
}