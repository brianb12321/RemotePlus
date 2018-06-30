using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BetterLogger;
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
            ServerManager.Logger.Log("Registering math commands.", LogLevel.Info, builder.FriendlyName);
            ServerManager.ServerRemoteService.Commands.Add("abs", MathCommands.abs);
            ServerManager.ServerRemoteService.Commands.Add("pow", MathCommands.pow);
        }
    }
}
