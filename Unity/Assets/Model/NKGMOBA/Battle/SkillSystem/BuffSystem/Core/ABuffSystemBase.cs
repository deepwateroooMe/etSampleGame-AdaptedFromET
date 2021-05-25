//------------------------------------------------------------
// Author: 烟雨迷离半世殇
// Mail: 1778139321@qq.com
// Data: 2019年9月16日 21:39:43
//------------------------------------------------------------

using Sirenix.OdinInspector;

namespace ETModel
{
    public abstract class ABuffSystemBase: IReference
    {
        /// <summary>
        /// 归属的运行时行为树实例
        /// </summary>
        public NP_RuntimeTree BelongtoRuntimeTree;

        /// <summary>
        /// Buff当前状态
        /// </summary>
        public BuffState BuffState;

        /// <summary>
        /// 当前叠加数
        /// </summary>
        public int CurrentOverlay;

        /// <summary>
        /// 最多持续到什么时候
        /// </summary>
        public long MaxLimitTime;

        /// <summary>
        /// Buff数据
        /// </summary>
        public BuffDataBase BuffData;

        /// <summary>
        /// 来自哪个Unit
        /// </summary>
        [DisableInEditorMode]
        public Unit TheUnitFrom;

        /// <summary>
        /// 寄生于哪个Unit，并不代表当前Buff实际寄居者，需要通过GetBuffTarget来获取，因为它赋值于Buff链起源的地方，具体值取决于那个起源Buff
        /// </summary>
        [DisableInEditorMode]
        public Unit TheUnitBelongto;

        /// <summary>
        /// 初始化buff数据
        /// </summary>
        /// <param name="buffData">Buff数据</param>
        /// <param name="theUnitFrom">来自哪个Unit</param>
        /// <param name="theUnitBelongto">寄生于哪个Unit</param>
        public virtual void OnInit(BuffDataBase buffData, Unit theUnitFrom, Unit theUnitBelongto)
        {
            //设置Buff来源Unit和归属Unit
            this.TheUnitFrom = theUnitFrom;
            this.TheUnitBelongto = theUnitBelongto;
            this.BuffData = buffData;
            BuffTimerAndOverlayHelper.CalculateTimerAndOverlay(this, this.BuffData);
        }

        /// <summary>
        /// Buff触发
        /// </summary>
        public abstract void OnExecute();

        /// <summary>
        /// Buff持续
        /// </summary>
        public virtual void OnUpdate()
        {
        }

        /// <summary>
        /// 重置Buff用
        /// </summary>
        public virtual void OnFinished()
        {
            
        }

        /// <summary>
        /// 刷新，用于刷新Buff状态
        /// </summary>
        public virtual void OnRefresh()
        {
        }

        public void Clear()
        {
            BelongtoRuntimeTree = null;
            BuffState = BuffState.Waiting;
            CurrentOverlay = 0;
            MaxLimitTime = 0;
            BuffData = null;
            TheUnitFrom = null;
            TheUnitBelongto = null;
        }
    }
}