using RemotePlusServer.Core;
using BetterLogger;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Core.EventSystem.Events;
using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.Scripting;
using System;
using RemotePlusLibrary.Extension;
using RemotePlusServer;

namespace WindowsTools
{
    public class Startup : ILibraryStartup
    {
        public void Init(IServiceCollection services)
        {
            IScriptExecutionContext context = IOCContainer.GetService<IScriptingEngine>().GetDefaultModule();
            context.AddVariable<Func<long, string>>("randChars", ScriptFunctions.randChars);
        }

        public void PostInit()
        {
            GlobalServices.EventBus.Subscribe<LoginEvent>(e => e.ClientLoggedIn.GetClient<RemoteClient>().ClientCallback.TellMessageToServerConsole("Welcome to WindowsTools!"), e => e.LoginSuccessful);
        }
    }
}