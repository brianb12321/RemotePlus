using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BetterLogger;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.Core;
using System.Reflection;
using ProxyServer.Scripting;
using ProxyServer.Scripting.Batch;

namespace ProxyServer
{
    public static class ServerBuilderExtensions
    {
        public static IServerBuilder InitializeScriptingEngine(this IServerBuilder builder)
        {
            return builder.AddTask(() =>
            {
                GlobalServices.Logger.Log("Starting scripting engine.", LogLevel.Info);
                ProxyManager.ScriptBuilder.InitializeEngine();
                GlobalServices.Logger.Log($"Engine started. IronPython version {ProxyManager.ScriptBuilder.ScriptingEngine.LanguageVersion.ToString()}", LogLevel.Info, "Scripting Engine");
                GlobalServices.Logger.Log("Redirecting STDOUT to duplex channel.", LogLevel.Debug, "Scripting Engine");
                ProxyManager.ScriptBuilder.ScriptingEngine.Runtime.IO.SetOutput(new MemoryStream(), new Internal._ClientTextWriter());
                //ServerManager.ScriptBuilder.ScriptingEngine.Runtime.IO.SetInput(new MemoryStream(), new Internal._ClientTextReader(), Encoding.ASCII);
                GlobalServices.Logger.Log("Finished starting scripting engine.", LogLevel.Info);
            });
        }
        public static IServerBuilder InitializeGlobals(this IServerBuilder builder)
        {
            return builder.AddTask(() =>
            {
                GlobalServices.Logger.Log("Initializing functions and variables.", LogLevel.Info, "Scripting Engine");
                InitializeGlobals();
                ProxyManager.ScriptBuilder.AddAssembly("ProxyServer");
                ProxyManager.ScriptBuilder.AddClass<BatchJob>();
                ProxyManager.ScriptBuilder.AddClass<TaskGroup>();
                ProxyManager.ScriptBuilder.AddClass<JobTask>();
                ProxyManager.ScriptBuilder.AddClass<JobExecutionMode>();
            });
        }

        private static void InitializeGlobals()
        {
            ProxyManager.ScriptBuilder.AddScriptObject("switchServer", new Action<int>(server =>
            {
                ProxyManager.ProxyService.RemoteInterface.SelectServer(server);
            }), "Sets the active server to the specified server index.", RemotePlusLibrary.Scripting.ScriptGlobalType.Function);
            ProxyManager.ScriptBuilder.AddScriptObject("executeProxyCommand", new Func<string, CommandPipeline>(command =>
            {
                return ProxyManager.ProxyService.RemoteInterface.ExecuteProxyCommand(command, RemotePlusLibrary.Extension.CommandSystem.CommandExecutionMode.Script);
            }), "Sends the specified command to the proxy server.", RemotePlusLibrary.Scripting.ScriptGlobalType.Function);
            ProxyManager.ScriptBuilder.AddScriptObject("runScriptOnServer", new Func<string, bool>(script =>
            {
                return ProxyManager.ProxyService.RemoteInterface.ExecuteScript(script);
            }), "Runs a script to the selected server.", RemotePlusLibrary.Scripting.ScriptGlobalType.Function);
            ProxyManager.ScriptBuilder.AddScriptObject("getServers", new Func<Guid[]>(() =>
            {
                return ProxyManager.ProxyService.RemoteInterface.ConnectedServers.Select(s => s.UniqueID).ToArray();
            }), "Returns an array of all the connected servers", RemotePlusLibrary.Scripting.ScriptGlobalType.Function);
        }
    }
}