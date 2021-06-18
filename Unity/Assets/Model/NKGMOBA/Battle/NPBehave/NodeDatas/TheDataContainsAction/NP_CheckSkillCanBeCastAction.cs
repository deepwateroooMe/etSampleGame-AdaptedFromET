//------------------------------------------------------------
// Author: 烟雨迷离半世殇
// Mail: 1778139321@qq.com
// Data: 2019年9月7日 10:29:38
//------------------------------------------------------------

using System;
using ETModel.NKGMOBA.Battle.State;
using Sirenix.OdinInspector;

namespace ETModel
{
    /// <summary>
    /// 检查技能是否能释放
    /// </summary>
    [Title("检查技能是否能释放", TitleAlignment = TitleAlignments.Centered)]
    public class NP_CheckSkillCanBeCastAction: NP_ClassForStoreAction
    {
        [BoxGroup("检测相关")]
        [LabelText("引用的数据结点Id")]
        public VTD_Id DataId = new VTD_Id();

        [BoxGroup("检测相关")]
        [LabelText("检查技能的Id")]
        public VTD_Id SkillIdBelongTo = new VTD_Id();

        [HideInEditorMode]
        public SkillDesNodeData SkillDesNodeData;
        
        [BoxGroup("检测相关")]
        [LabelText("互斥的状态")]
        public StateTypes ConfliectType;
        
        public override Func<bool> GetFunc1ToBeDone()
        {
            this.Func1 = this.CheckCostToSpanSkill;
            return this.Func1;
        }

        private bool CheckCostToSpanSkill()
        {
            this.SkillDesNodeData = (SkillDesNodeData) this.BelongtoRuntimeTree.BelongNP_DataSupportor.BuffNodeDataDic[this.DataId.Value];
            // 相关状态检测，例如沉默，眩晕等,下面是示例代码

            if (Game.Scene.GetComponent<UnitComponent>().Get(this.Unitid).GetComponent<StackFsmComponent>()
                    .CheckConflictState(ConfliectType))
            {
                return false;
            }
            
            //给要修改的黑板节点进行赋值
            UnitAttributesDataComponent unitAttributesDataComponent = UnitComponent.Instance.Get(this.Unitid).GetComponent<UnitAttributesDataComponent>();
            switch (this.SkillDesNodeData.SkillCostTypes)
            {
                case SkillCostTypes.MagicValue:
                    //依据技能具体消耗来进行属性改变操作
                    return true;
                case SkillCostTypes.Other:
                    return true;
                case SkillCostTypes.HPValue:
                    return true;
                default:
                    return true;
            }
        }
    }
}