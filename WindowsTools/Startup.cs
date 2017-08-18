using RemotePlusLibrary.Extension;
using RemotePlusLibrary.Extension.ExtensionLibraries.LibraryStartupTypes;
using RemotePlusServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsTools
{
    public class Startup : ILibraryStartup
    {
        public void Init(ILibraryBuilder builder)
        {
            ServerManager.Logger.AddOutput("Adding OS commands", Logging.OutputLevel.Info, "WindowsTools");
            ServerManager.DefaultService.Commands.Add("sendKey", OSCommands.sendKey);
        }
    }
}
