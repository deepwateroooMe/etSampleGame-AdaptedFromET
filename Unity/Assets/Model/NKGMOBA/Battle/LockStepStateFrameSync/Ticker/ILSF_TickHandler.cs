namespace ET {
    public interface ILSF_TickHandler {
        // 当前帧Tick开始，可用于做数据准备工作
        void LSF_TickStart(Entity entity, uint frame, long deltaTime);
        // 正常Tick
        void LSF_Tick(Entity entity, uint currentFrame, long deltaTime);
        // 当前帧所有Tick都结束了，可用于做数据收集工作
        void LSF_TickEnd(Entity entity, uint frame, long deltaTime);
#if !SERVER
        // 检测结果一致性
        bool LSF_CheckConsistency(Entity entity, uint frame, ALSF_Cmd stateToCompare);
        // 视图层Tick
        void LSF_ViewTick(Entity entity, long deltaTime);
        // 回滚
        void LSF_RollBackTick(Entity entity, uint frame, ALSF_Cmd stateToCompare);
#endif
    }
}