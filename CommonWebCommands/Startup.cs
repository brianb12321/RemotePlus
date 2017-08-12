using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Extension.ExtensionLibraries.LibraryStartupTypes;
using RemotePlusServer;
using RemotePlusLibrary.Extension;

namespace CommonWebCommands
{
    public class Startup : ILibraryStartup
    {
        public void Init(LibraryBuilder builder)
        {
            ServerManager.Logger.AddOutput("Welcome to WebCommands.", Logging.OutputLevel.Info, "WebCommands");
            ServerManager.Logger.AddOutput("Adding Chrome", Logging.OutputLevel.Info, "WebCommands");
            ServerManager.DefaultService.Commands.Add("chrome", WebCommands.chrome);
            ServerManager.Logger.AddOutput("Adding Internet Explore", Logging.OutputLevel.Info, "WebCommands");
            ServerManager.DefaultService.Commands.Add("ie", WebCommands.ie);
            ServerManager.Logger.AddOutput("Adding downloadString command", Logging.OutputLevel.Info, "WebCommands");
            ServerManager.DefaultService.Commands.Add("downloadString", DownloadCommands.downloadWeb);
        }
    }
}
