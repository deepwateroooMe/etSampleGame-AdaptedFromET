﻿using FairyGUI;

namespace ETModel
{
    [ObjectSystem]
    public class FUIInitComponentAwakeSystem: AwakeSystem<FUIInitComponent>
    {
        public override void Awake(FUIInitComponent self)
        {
            self.Awake();
        }
    }

    /// <summary>
    /// 初始化Model层的检查资源更新UI，以及加载圆圈
    /// </summary>
    public class FUIInitComponent: Component
    {
        //暂时不需要设置字体
        //public const string DefaultFont = "FZXuanZhenZhuanBianS-R-GB";
        public static string CheckForResUpdatePackageName = "FUI/FUICheckForResUpdate";
        public static string LoadingPackageName = "FUI/FUILoading";
        private UIPackage checkForResUpdatelPackage;
        private UIPackage loadingPackage;

        public void Awake()
        {
            //对于FGUI来说，其内部在执行 `UIPackage.RemovePackage` 时会进行`ab.Unload(true)`操作，应该是个很贴心的设计，但我们xasset需要管理资源的引用计数，所以不需要这个贴心的功能，故：
            UIPackage.unloadBundleByFGUI = false;
            //UIConfig.defaultFont = DefaultFont;
            this.checkForResUpdatelPackage = UIPackage.AddPackage(CheckForResUpdatePackageName);
            FUICheckForResUpdateBinder.BindAll();
            this.loadingPackage = UIPackage.AddPackage(LoadingPackageName);
            FUILoadingBinder.BindAll();
        }

        public override void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            base.Dispose();

            if (this.checkForResUpdatelPackage != null)
            {
                var p = UIPackage.GetByName(checkForResUpdatelPackage.name);

                if (p != null)
                {
                    UIPackage.RemovePackage(checkForResUpdatelPackage.name);
                }

                checkForResUpdatelPackage = null;
            }

            if (this.loadingPackage != null)
            {
                var p = UIPackage.GetByName(loadingPackage.name);

                if (p != null)
                {
                    UIPackage.RemovePackage(loadingPackage.name);
                }

                loadingPackage = null;
            }
        }
    }
}