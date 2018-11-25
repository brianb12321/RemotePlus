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
            GlobalServices.Logger.Log("Welcome to WebCommands.", LogLevel.Info, "WebCommand");
            builder.AddCommandClass<WebCommands>();
            builder.AddCommandClass<DownloadCommands>();
        }
    }
}