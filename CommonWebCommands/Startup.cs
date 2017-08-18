using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Extension.ExtensionLibraries.LibraryStartupTypes;
using RemotePlusServer;
using System.IO;
using RemotePlusLibrary.Extension;

namespace CommonWebCommands
{
    public class Startup : ILibraryStartup
    {
        public void Init(ILibraryBuilder builder)
        {
            ServerManager.Logger.AddOutput("Welcome to WebCommands.", Logging.OutputLevel.Info, "WebCommands");
            ServerManager.Logger.AddOutput("Adding Chrome", Logging.OutputLevel.Info, "WebCommands");
            CheckChrome();
            ServerManager.DefaultService.Commands.Add("chrome", WebCommands.chrome);
            ServerManager.Logger.AddOutput("Adding Internet Explore", Logging.OutputLevel.Info, "WebCommands");
            CheckIE();
            ServerManager.DefaultService.Commands.Add("ie", WebCommands.ie);
            ServerManager.Logger.AddOutput("Adding Opera", Logging.OutputLevel.Info, "WebCommands");
            CheckOpera();
            ServerManager.DefaultService.Commands.Add("opera", WebCommands.opera);
            ServerManager.Logger.AddOutput("Adding Firefox", Logging.OutputLevel.Info, "WebCommands");
            CHeckFirefox();
            ServerManager.DefaultService.Commands.Add("firefox", WebCommands.firefox);
        }
        void CheckIE()
        {
            if(!File.Exists(@"C:\Program Files\Internet Explorer\iexplore.exe"))
            {
                ServerManager.Logger.AddOutput(new Logging.LogItem(Logging.OutputLevel.Warning, "Internet explore is not installed on the server.", "WebCommands") { Color = ConsoleColor.Yellow });
            }
        }
        void CheckChrome()
        {
            if(!File.Exists(@"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe"))
            {
                ServerManager.Logger.AddOutput(new Logging.LogItem(Logging.OutputLevel.Warning, "Chrome is not installed on the server.", "WebCommands") { Color = ConsoleColor.Yellow });
            }
        }
        void CHeckFirefox()
        {
            if(!File.Exists(@"C:\Program Files (x86)\Mozilla Firefox\firefox.exe"))
            {
                ServerManager.Logger.AddOutput(new Logging.LogItem(Logging.OutputLevel.Warning, "Firefox is not installed on the server.", "WebCommands") { Color = ConsoleColor.Yellow });
            }
        }
        void CheckOpera()
        {
            if(!File.Exists(@"C:\Program Files\Opera\launcher.exe"))
            {
                ServerManager.Logger.AddOutput(new Logging.LogItem(Logging.OutputLevel.Warning, "Opera is not installed on the server.", "WebCommands") { Color = ConsoleColor.Yellow });
            }
        }
    }
}