using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BetterLogger;
using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.Core;
using System.Reflection;
using ProxyServer.Scripting;
using ProxyServer.Scripting.Batch;
using RemotePlusLibrary.Scripting;
using RemotePlusLibrary.SubSystem.Command.CommandClasses;
using RemotePlusLibrary.SubSystem.Command;

namespace ProxyServer
{
    public static class ServerBuilderExtensions
    {
        public static IServerBuilder InitializeGlobals(this IServerBuilder builder)
        {
            return builder.AddTask(() =>
            {
                GlobalServices.Logger.Log("Initializing functions and variables.", LogLevel.Info, "Scripting Engine");
                InitializeGlobals();
            });
        }

        private static void InitializeGlobals()
        {
            IScriptExecutionContext context = IOCContainer.GetService<IScriptingEngine>().GetDefaultModule();
            context.AddVariable("switchServer", new Action<int>(server =>
            {
                ProxyManager.ProxyService.RemoteInterface.SelectServer(server);
            }));
            context.AddVariable("executeProxyCommand", new Func<string, CommandPipeline>(command =>
            {
                return ProxyManager.ProxyService.RemoteInterface.ExecuteProxyCommand(command, CommandExecutionMode.Script);
            }));
            context.AddVariable("runScriptOnServer", new Func<string, object>(script =>
            {
                return ProxyManager.ProxyService.RemoteInterface.ExecuteScript(script);
            }));
            context.AddVariable("getServers", new Func<Guid[]>(() =>
            {
                return ProxyManager.ProxyService.RemoteInterface.ConnectedServers.Select(s => s.UniqueID).ToArray();
            }));
        }
    }
}