//------------------------------------------------------------
// 此代码由工具自动生成，请勿更改
// 此代码由工具自动生成，请勿更改
// 此代码由工具自动生成，请勿更改
//------------------------------------------------------------

using ETModel;
using GraphProcessor;
using UnityEditor;

namespace Plugins.NodeEditor
{
    [NodeMenuItem("技能数据部分/播放特效Buff", typeof (SkillGraph))]
    public class PlayEffectBuffNode: BuffNodeBase
    {
        public override string name => "播放特效Buff";

        public NormalBuffNodeData SkillBuffBases =
                new NormalBuffNodeData()
                {
                    BuffDes = "播放特效Buff",
                    BuffData = new PlayEffectBuffData() { BelongBuffSystemType = BuffSystemType.PlayEffectBuffSystem }
                };

        public override BuffNodeDataBase Skill_GetNodeData()
        {
            return SkillBuffBases;
        }
    }
}
