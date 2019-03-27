using System.IO;
using RemotePlusLibrary.RequestSystem;
using RemotePlusServer.Core;
using BetterLogger;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Extension;
using RemotePlusLibrary.Core.IOC;

namespace CommonWebCommands
{
    public class Startup : ILibraryStartup
    {
        public void Init(IServiceCollection services)
        {
            GlobalServices.Logger.Log("Welcome to WebCommands.", LogLevel.Info, "WebCommand");
        }

        public void PostInit()
        {
            
        }
    }
}