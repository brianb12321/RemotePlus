using BetterLogger;
using RemotePlusLibrary;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using RemotePlusLibrary.ServiceArchitecture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyServer
{
    public class ProxyCommands : ICommandClass
    {
        public Dictionary<string, CommandDelegate> Commands { get; } = new Dictionary<string, CommandDelegate>();
        ILogFactory _logger;
        IRemotePlusService<ProxyServerRemoteImpl> _service;
        ICommandClassStore _store;
        public ProxyCommands(ILogFactory logger, IRemotePlusService<ProxyServerRemoteImpl> service, ICommandClassStore store)
        {
            _logger = logger;
            _service = service;
            _store = store;
        }

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
                _service.RemoteInterface.ProxyClient.ClientCallback.TellMessageToServerConsole(_service.RemoteInterface.GetSelectedServerGuid(), "The specifed server index does not exist.", LogLevel.Error);
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
            _service.RemoteInterface.ProxyClient.ClientCallback.TellMessageToServerConsole(ProxyManager.ProxyGuid, sb.ToString());
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Shows help screen.")]
        public CommandResponse help(CommandRequest req, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            string helpString = string.Empty;
            if (req.Arguments.Count == 2)
            {
                helpString = RemotePlusConsole.ShowHelpPage(_store.GetAllCommands(), req.Arguments[1].Value.ToString());
            }
            else
            {
                helpString = RemotePlusConsole.ShowHelp(_store.GetAllCommands());
            }
            _service.RemoteInterface.ProxyClient.ClientCallback.TellMessageToServerConsole(ProxyManager.ProxyGuid, helpString);
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
            ProxyManager.ScriptBuilder.ClearStaticScope();
            return new CommandResponse((int)CommandStatus.Success);
        }

        public void AddCommands()
        {
            Commands.Add("proxySwitchServer", switchServer);
            Commands.Add("proxyHelp", help);
            Commands.Add("proxyViewServers", viewServers);
            Commands.Add("proxyRegister", register);
            Commands.Add("proxyResetStaticScript", resetStaticScript);
        }

        public CommandDelegate Lookup(string commandName)
        {
            if (Commands.TryGetValue(commandName, out CommandDelegate command))
            {
                return command;
            }
            else
            {
                return null;
            }
        }

        public bool HasCommand(string commandName)
        {
            return Commands.ContainsKey(commandName);
        }
    }
}
