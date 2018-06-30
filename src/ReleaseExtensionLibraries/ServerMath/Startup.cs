using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Extension;
using RemotePlusLibrary.Extension.ExtensionLoader.Initialization;
using RemotePlusServer;
using RemotePlusServer.Core;

namespace ServerMath
{
    public class Startup : ILibraryStartup
    {
        public void Init(ILibraryBuilder builder, IInitEnvironment env)
        {
            ServerManager.Logger.AddOutput("Registering math commands.", Logging.OutputLevel.Info, builder.FriendlyName);
            ServerManager.ServerRemoteService.Commands.Add("abs", MathCommands.abs);
            ServerManager.ServerRemoteService.Commands.Add("pow", MathCommands.pow);
        }
    }
}
