using RemotePlusLibrary.Extension.ExtensionLoader.Initialization;
using RemotePlusServer.Core;
using BetterLogger;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Core.EventSystem.Events;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.Scripting;
using System;

namespace WindowsTools
{
    public class Startup : ILibraryStartup
    {
        public void Init(ILibraryBuilder builder, IInitEnvironment env)
        {
            builder.AddCommandClass<OSCommands>();
            GlobalServices.Logger.Log($"Init position {env.InitPosition}", LogLevel.Debug, "WindowsTools");
            IScriptingEngine engine = IOCContainer.GetService<IScriptingEngine>();
            engine.AddScriptObject<Func<long, string>>("randChars", ScriptFunctions.randChars, "Generates a random viewable ASCI character.", ScriptGlobalType.Function);
        }

        public void PostInit()
        {
            GlobalServices.EventBus.Subscribe<LoginEvent>(e => ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole("Welcome to WindowsTools!"), e => e.LoginSuccessful);
        }
    }
}