using ETModel;

namespace ETHotfix
{
    [Event(EventIdType.ShowLoginUI)]
    public class ShowLoginUI_CreateLoginUI: AEvent
    {
        public override void Run()
        {
            var loginui = FUILogin.CreateInstance();
            //默认将会以Id为Name，也可以自定义Name，方便查询和管理
            loginui.Name = FUIPackage.FUILogin;
            loginui.GObject.sortingOrder = 1000;
            loginui.self.fairyBatching = true;
            loginui.MakeFullScreen();
            Game.Scene.GetComponent<FUIComponent>().Add(loginui, true);
            loginui.AddComponent<FUILoginComponent>();
        }
    }

    [Event(EventIdType.LobbyUIAllDataLoadComplete)]
    public class LoginSuccess_CloseLoginUI: AEvent
    {
        public override void Run()
        {
            Game.Scene.GetComponent<FUIComponent>().Remove(FUIPackage.FUILogin);
        }
    }
}