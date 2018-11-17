using System.IO;
using RemotePlusLibrary.Extension.ExtensionLoader.Initialization;
using RemotePlusLibrary.RequestSystem;
using RemotePlusServer.Core;
using BetterLogger;
using RemotePlusLibrary.Core;

namespace CommonWebCommands
{
    public class Startup : ILibraryStartup
    {
        public void Init(ILibraryBuilder builder, IInitEnvironment env)
        {
            GlobalServices.Logger.Log($"Current position {env.InitPosition}", LogLevel.Debug, "WebCommands");
            GlobalServices.Logger.Log("Welcome to WebCommands.", LogLevel.Info, "WebCommands");
            GlobalServices.Logger.Log("Adding Chrome", LogLevel.Info, "WebCommands");
            CheckChrome();
            ServerManager.ServerRemoteService.Commands.Add("chrome", WebCommands.chrome);
            GlobalServices.Logger.Log("Adding Internet Explore", LogLevel.Info, "WebCommands");
            CheckIE();
            ServerManager.ServerRemoteService.Commands.Add("ie", WebCommands.ie);
            GlobalServices.Logger.Log("Adding Opera", LogLevel.Info, "WebCommands");
            CheckOpera();
            ServerManager.ServerRemoteService.Commands.Add("opera", WebCommands.opera);
            GlobalServices.Logger.Log("Adding Firefox", LogLevel.Info, "WebCommands");
            CHeckFirefox();
            ServerManager.ServerRemoteService.Commands.Add("firefox", WebCommands.firefox);
        }
        void CheckIE()
        {
            if(!File.Exists(@"C:\Program Files\Internet Explorer\iexplore.exe"))
            {
                GlobalServices.Logger.Log("Internet explore is not installed on the server.", LogLevel.Warning, "WebCommands");
            }
        }
        void CheckChrome()
        {
            if(!File.Exists(@"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe"))
            {
                GlobalServices.Logger.Log("Chrome is not installed on the server.", LogLevel.Warning, "WebCommands");
            }
        }
        void CHeckFirefox()
        {
            if(!File.Exists(@"C:\Program Files (x86)\Mozilla Firefox\firefox.exe"))
            {
                GlobalServices.Logger.Log("Firefox is not installed on the server.", LogLevel.Warning, "WebCommands");
            }
        }
        void CheckOpera()
        {
            if(!File.Exists(@"C:\Program Files\Opera\launcher.exe"))
            {
                GlobalServices.Logger.Log("Opera is not installed on the server.", LogLevel.Warning, "WebCommands");
            }
        }
    }
}