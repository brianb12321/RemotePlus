using BetterLogger;
using Ninject;
using ProxyServer.ExtensionSystem;
using RemotePlusLibrary;
using RemotePlusLibrary.Extension;
using RemotePlusLibrary.Scripting;
using RemotePlusLibrary.ServiceArchitecture;
using RemotePlusLibrary.SubSystem.Command;
using RemotePlusLibrary.SubSystem.Command.CommandClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Core.IOC;

namespace ProxyServer
{
    [ExtensionModule]
    public class ProxyCommands : ProxyCommandClass
    {
        ILogFactory _logger;
        IRemotePlusService<ProxyServerRemoteImpl> _service;
        ICommandSubsystem<IProxyCommandModule> _commandSubsystem;
        IScriptingEngine _scriptEngine;

        [CommandHelp("Switches the specified server into the active server.")]
        public CommandResponse switchServer(CommandRequest req, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            if (int.TryParse(req.Arguments[1].Value.ToString(), out int result))
            {
                _service.RemoteInterface.SelectServer(result);
                return new CommandResponse((int)CommandStatus.Success);
            }
            else
            {
                _service.RemoteInterface.ProxyClient.ClientCallback.WriteToClientConsole(_service.RemoteInterface.GetSelectedServerGuid(), "The specifed server index does not exist.", LogLevel.Error);
                return new CommandResponse((int)CommandStatus.Fail);
            }
        }
        [CommandHelp("Lists all the servers connected to the proxy.")]
        public CommandResponse viewServers(CommandRequest req, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < _service.RemoteInterface.ConnectedServers.Count; i++)
            {
                sb.AppendLine($"Index: {i}, GUID: {_service.RemoteInterface.ConnectedServers[i].UniqueID}");
            }
            _service.RemoteInterface.ProxyClient.ClientCallback.WriteToClientConsole(ProxyManager.ProxyGuid, sb.ToString());
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Shows help screen.")]
        public CommandResponse help(CommandRequest req, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            string helpString = string.Empty;
            if (req.Arguments.Count == 2)
            {
                helpString = _commandSubsystem.ShowHelpPage(req.Arguments[1].Value.ToString());
            }
            else
            {
                helpString = _commandSubsystem.ShowHelpScreen();
            }
            _service.RemoteInterface.ProxyClient.ClientCallback.WriteToClientConsole(ProxyManager.ProxyGuid, helpString);
            var response = new CommandResponse((int)CommandStatus.Success);
            response.ReturnData = helpString;
            return response;
        }
        [CommandHelp("Establish registration with the selected server.")]
        public CommandResponse register(CommandRequest req, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            _service.RemoteInterface.Register(new RegisterationObject());
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Clears all variables and functions from the interactive scripts.")]
        public CommandResponse resetStaticScript(CommandRequest reqest, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            _scriptEngine.ResetSessionContext();
            return new CommandResponse((int)CommandStatus.Success);
        }
        public override void InitializeServices(IServiceCollection services)
        {
            _logger = services.GetService<ILogFactory>();
            _service = services.GetService<IRemotePlusService<ProxyServerRemoteImpl>>();
            _commandSubsystem = services.GetService<ICommandSubsystem<IProxyCommandModule>>();
            _scriptEngine = services.GetService<IScriptingEngine>();
            Commands.Add("proxySwitchServer", switchServer);
            Commands.Add("proxyHelp", help);
            Commands.Add("proxyViewServers", viewServers);
            Commands.Add("proxyRegister", register);
            Commands.Add("proxyResetStaticScript", resetStaticScript);
        }
    }
}