//------------------------------------------------------------
// Author: 烟雨迷离半世殇
// Mail: 1778139321@qq.com
// Data: 2019年7月3日 17:44:51
//------------------------------------------------------------

using System;
using ETModel;

namespace ETHotfix
{
    [ObjectSystem]
    public class PlayerHeroControllerAwakeSystem: AwakeSystem<PlayerHeroControllerComponent>
    {
        public override void Awake(PlayerHeroControllerComponent self)
        {
            self.Awake();
        }
    }

    [ObjectSystem]
    public class PlayerHeroControllerUpdateSystem: UpdateSystem<PlayerHeroControllerComponent>
    {
        public override void Update(PlayerHeroControllerComponent self)
        {
            self.Update();
        }
    }

    /// <summary>
    /// 玩家自己操控英雄组件
    /// </summary>
    public class PlayerHeroControllerComponent: Component
    {
        private UserInputComponent userInputComponent;

        public void Awake()
        {
            this.userInputComponent = ETModel.Game.Scene.GetComponent<UserInputComponent>();
        }

        public void Update()
        {
            if (this.userInputComponent.QDown)
            {
                SessionComponent.Instance.Session.Send(new UserInput_SkillCmd() { Message = "Q" });
            }

            if (this.userInputComponent.WDown)
            {
                SessionComponent.Instance.Session.Send(new UserInput_SkillCmd() { Message = "W" });
            }

            if (this.userInputComponent.EDown)
            {
                SessionComponent.Instance.Session.Send(new UserInput_SkillCmd() { Message = "E" });
            }

            if (this.userInputComponent.RDown)
            {
                SessionComponent.Instance.Session.Send(new UserInput_SkillCmd() { Message = "R" });
            }
        }
    }
}