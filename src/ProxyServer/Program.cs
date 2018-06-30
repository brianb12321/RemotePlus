using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary;
using Logging;
using System.Reflection;
using RemotePlusLibrary.Discovery;
using System.Windows.Forms;
using System.ServiceModel.Dispatcher;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using RemotePlusLibrary.Extension;
using RemotePlusLibrary.Client;
using BetterLogger;
using BetterLogger.Loggers;

namespace ProxyServer
{
    class ProxyManager
    {
        public static ILogFactory Logger { get; } = new BaseLogFactory();
        public static Guid ProxyGuid { get; } = Guid.NewGuid();
        public static IRemotePlusService<ProxyServerRemoteImpl> ProxyService { get; set; }
        [STAThread]
        static void Main(string[] args)
        {
            Logger.AddLogger(new ConsoleLogger());
            var a = Assembly.GetExecutingAssembly().GetName();
            Console.WriteLine($"Welcome to {a.Name}, version: {a.Version.ToString()}\n\n");
            CreateProxyServer();
            InitializeCommands();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ServerControls(false));
        }
        [CommandHelp("Switches the specified server into the active server.")]
        static CommandResponse switchServer(CommandRequest req, CommandPipeline pipe)
        {
            if (int.TryParse(req.Arguments[1].Value, out int result))
            {
                ProxyService.RemoteInterface.SelectServer(result);
                return new CommandResponse((int)CommandStatus.Success);
            }
            else
            {
                ProxyService.RemoteInterface.ProxyClient.ClientCallback.TellMessageToServerConsole(ProxyService.RemoteInterface.GetSelectedServerGuid(), "The specifed server index does not exist.", LogLevel.Error);
                return new CommandResponse((int)CommandStatus.Fail);
            }
        }
        [CommandHelp("Lists all the servers connected to the proxy.")]
        static CommandResponse viewServers(CommandRequest req, CommandPipeline pipe)
        {
            StringBuilder sb = new StringBuilder();
            for(int i = 0; i < ProxyService.RemoteInterface.ConnectedServers.Count; i++)
            {
                sb.AppendLine($"Index: {i}, GUID: {ProxyService.RemoteInterface.ConnectedServers[i].UniqueID}");
            }
            ProxyService.RemoteInterface.ProxyClient.ClientCallback.TellMessageToServerConsole(ProxyGuid, sb.ToString());
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Shows help screen.")]
        static CommandResponse help(CommandRequest req, CommandPipeline pipe)
        {
            var helpString = RemotePlusConsole.ShowHelp(ProxyService.Commands, req.Arguments.Select(t => t.Value).ToArray());
            ProxyService.RemoteInterface.ProxyClient.ClientCallback.TellMessageToServerConsole(ProxyGuid, helpString);
            ProxyService.RemoteInterface.ProxyClient.ClientCallback.TellMessageToServerConsole(ProxyGuid, "\nAny commands not listed on this list will be executed on the selected server.\n");
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Establish registeration with the selected server.")]
        static CommandResponse register(CommandRequest req, CommandPipeline pipe)
        {
            ProxyService.RemoteInterface.Register(new RegisterationObject());
            return new CommandResponse((int)CommandStatus.Success);
        }

        private static void InitializeCommands()
        {
            ProxyService.Commands.Add("proxySwitchServer", switchServer);
            ProxyService.Commands.Add("proxyHelp", help);
            ProxyService.Commands.Add("proxyViewServers", viewServers);
            ProxyService.Commands.Add("proxyRegister", register);
        }

        internal static void RunInServerMode()
        {
            ProxyService.Start();
        }

        private static void CreateProxyServer()
        {
            Logger.Log("Opening proxy server.", LogLevel.Info);
            ProxyService = ProbeService.CreateProxyService(typeof(IProxyServerRemote), new ProxyServerRemoteImpl(),
                9001,
                "Proxy",
                "ProxyClient",
                (m, o) => Logger.Log(m, o), null);
            ProxyService.HostOpened += ProxyService_HostOpened;
            ProxyService.HostClosed += ProxyService_HostClosed;
            ProxyService.HostFaulted += ProxyService_HostFaulted;
        }

        internal static void Close()
        {
            ProxyService.Close();
            Environment.Exit(0);
        }

        private static void ProxyService_HostFaulted(object sender, EventArgs e)
        {
            Logger.Log("The proxy server state has been transferred to the faulted state.", LogLevel.Error);
        }

        private static void ProxyService_HostClosed(object sender, EventArgs e)
        {
            Logger.Log("Proxy server closed.", LogLevel.Info);
        }

        private static void ProxyService_HostOpened(object sender, EventArgs e)
        {
            Logger.Log($"Proxy server opened on port 9001", LogLevel.Info);
        }
        public static CommandResponse Execute(CommandRequest c, CommandExecutionMode commandMode, CommandPipeline pipe)
        {
            bool throwFlag = false;
            StatusCodeDeliveryMethod scdm = StatusCodeDeliveryMethod.DoNotDeliver;
            try
            {
                Logger.Log($"Executing server command {c.Arguments[0]}", LogLevel.Info);
                try
                {
                    var command = ProxyService.Commands[c.Arguments[0].Value];
                    var ba = RemotePlusConsole.GetCommandBehavior(command);
                    if (ba != null)
                    {
                        if (ba.TopChainCommand && pipe.Count > 0)
                        {
                            Logger.Log($"This is a top-level command.", LogLevel.Error);
                            ProxyService.RemoteInterface.ProxyClient.ClientCallback.TellMessage(ProxyGuid, $"This is a top-level command.", LogLevel.Error);
                            return new CommandResponse((int)CommandStatus.AccessDenied);
                        }
                        if (commandMode != ba.ExecutionType)
                        {
                            Logger.Log($"The command requires you to be in {ba.ExecutionType} mode.", LogLevel.Error);
                            ProxyService.RemoteInterface.ProxyClient.ClientCallback.TellMessage(ProxyGuid, $"The command requires you to be in {ba.ExecutionType} mode.", LogLevel.Error);
                            return new CommandResponse((int)CommandStatus.AccessDenied);
                        }
                        if (ba.SupportClients != ClientSupportedTypes.Both && ((ProxyService.RemoteInterface.ProxyClient.ClientType == ClientType.GUI && ba.SupportClients != ClientSupportedTypes.GUI) || (ProxyService.RemoteInterface.ProxyClient.ClientType == ClientType.CommandLine && ba.SupportClients != ClientSupportedTypes.CommandLine)))
                        {
                            if (string.IsNullOrEmpty(ba.ClientRejectionMessage))
                            {
                                Logger.Log($"Your client must be a {ba.SupportClients.ToString()} client.", LogLevel.Error);
                                ProxyService.RemoteInterface.ProxyClient.ClientCallback.TellMessage(ProxyGuid, $"Your client must be a {ba.SupportClients.ToString()} client.", LogLevel.Error);
                                return new CommandResponse((int)CommandStatus.UnsupportedClient);
                            }
                            else
                            {
                                Logger.Log(ba.ClientRejectionMessage, LogLevel.Error);
                                ProxyService.RemoteInterface.ProxyClient.ClientCallback.TellMessage(ProxyGuid, ba.ClientRejectionMessage, LogLevel.Error);
                                return new CommandResponse((int)CommandStatus.UnsupportedClient);
                            }
                        }
                        if (ba.DoNotCatchExceptions)
                        {
                            throwFlag = true;
                        }
                        if (ba.StatusCodeDeliveryMethod != StatusCodeDeliveryMethod.DoNotDeliver)
                        {
                            scdm = ba.StatusCodeDeliveryMethod;
                        }
                    }
                    Logger.Log("Found command, and executing.", LogLevel.Debug);
                    var sc = command(c, pipe);
                    if (scdm == StatusCodeDeliveryMethod.TellMessage)
                    {
                        ProxyService.RemoteInterface.ProxyClient.ClientCallback.TellMessage(ProxyGuid, $"Command {c.Arguments[0]} finished with status code {sc.ToString()}", LogLevel.Info);
                    }
                    else if (scdm == StatusCodeDeliveryMethod.TellMessageToServerConsole)
                    {
                        ProxyService.RemoteInterface.ProxyClient.ClientCallback.TellMessageToServerConsole(ProxyGuid, $"Command {c.Arguments[0]} finished with status code {sc.ToString()}", LogLevel.Info);
                    }
                    return sc;
                }
                catch (KeyNotFoundException)
                {
                    Logger.Log($"Failed to find the command. Passing command to selected server [{ProxyService.RemoteInterface.SelectedClient.UniqueID}]", LogLevel.Debug);
                    return new CommandResponse(3131);
                }
            }
            catch (Exception ex)
            {
                if (throwFlag)
                {
                    throw;
                }
                else
                {
                    Logger.Log("command failed: " + ex.Message, LogLevel.Info);
                    ProxyService.RemoteInterface.ProxyClient.ClientCallback.TellMessageToServerConsole(ProxyGuid, "Error whie executing command: " + ex.Message, LogLevel.Error);
                    return new CommandResponse((int)CommandStatus.Fail);
                }
            }
        }
    }
}