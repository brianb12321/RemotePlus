using System.IO;
using RemotePlusLibrary.Extension.HookSystem;
using RemotePlusLibrary.Extension.ExtensionLoader.Initialization;
using RemotePlusLibrary.RequestSystem;
using RemotePlusServer.Core;
using BetterLogger;

namespace CommonWebCommands
{
    public class Startup : ILibraryStartup
    {
        public void Init(ILibraryBuilder builder, IInitEnvironment env)
        {
            //builder.RegisterHook(LibraryBuilder.LOGIN_HOOK, checkIfUserAcceptsDisclamer);
            ServerManager.Logger.Log($"Current position {env.InitPosition}", LogLevel.Debug, "WebCommands");
            ServerManager.Logger.Log("Welcome to WebCommands.", LogLevel.Info, "WebCommands");
            ServerManager.Logger.Log("Adding Chrome", LogLevel.Info, "WebCommands");
            CheckChrome();
            ServerManager.ServerRemoteService.Commands.Add("chrome", WebCommands.chrome);
            ServerManager.Logger.Log("Adding Internet Explore", LogLevel.Info, "WebCommands");
            CheckIE();
            ServerManager.ServerRemoteService.Commands.Add("ie", WebCommands.ie);
            ServerManager.Logger.Log("Adding Opera", LogLevel.Info, "WebCommands");
            CheckOpera();
            ServerManager.ServerRemoteService.Commands.Add("opera", WebCommands.opera);
            ServerManager.Logger.Log("Adding Firefox", LogLevel.Info, "WebCommands");
            CHeckFirefox();
            ServerManager.ServerRemoteService.Commands.Add("firefox", WebCommands.firefox);
        }
        void checkIfUserAcceptsDisclamer(HookArguments args)
        {
            ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.RequestInformation(RequestBuilder.RequestMessageBox("Do not use CommonWebCommands to cause harm!", "Warning", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning));
        }
        void CheckIE()
        {
            if(!File.Exists(@"C:\Program Files\Internet Explorer\iexplore.exe"))
            {
                ServerManager.Logger.Log("Internet explore is not installed on the server.", LogLevel.Warning, "WebCommands");
            }
        }
        void CheckChrome()
        {
            if(!File.Exists(@"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe"))
            {
                ServerManager.Logger.Log("Chrome is not installed on the server.", LogLevel.Warning, "WebCommands");
            }
        }
        void CHeckFirefox()
        {
            if(!File.Exists(@"C:\Program Files (x86)\Mozilla Firefox\firefox.exe"))
            {
                ServerManager.Logger.Log("Firefox is not installed on the server.", LogLevel.Warning, "WebCommands");
            }
        }
        void CheckOpera()
        {
            if(!File.Exists(@"C:\Program Files\Opera\launcher.exe"))
            {
                ServerManager.Logger.Log("Opera is not installed on the server.", LogLevel.Warning, "WebCommands");
            }
        }
    }
}