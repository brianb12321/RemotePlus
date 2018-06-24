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

namespace ProxyServer
{
    class ProxyManager
    {
        public static CMDLogging Logger { get; } = new CMDLogging();
        public static Guid ProxyGuid { get; } = Guid.NewGuid();
        public static ProbeService<ProxyServerRemoteImpl> ProxyService { get; set; }
        [STAThread]
        static void Main(string[] args)
        {
            Logger.DefaultFrom = "Proxy Server";
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
                ProxyService.Remote.SelectServer(result);
                return new CommandResponse((int)CommandStatus.Success);
            }
            else
            {
                ProxyService.Remote.ProxyClient.ClientCallback.TellMessageToServerConsole(ProxyService.Remote.GetSelectedServerGuid(), new UILogItem(OutputLevel.Error, "The specifed server index does not exist."));
                return new CommandResponse((int)CommandStatus.Fail);
            }
        }
        [CommandHelp("Lists all the servers connected to the proxy.")]
        static CommandResponse viewServers(CommandRequest req, CommandPipeline pipe)
        {
            StringBuilder sb = new StringBuilder();
            for(int i = 0; i < ProxyService.Remote.ConnectedServers.Count; i++)
            {
                sb.AppendLine($"Index: {i}, GUID: {ProxyService.Remote.ConnectedServers[i].UniqueID}");
            }
            ProxyService.Remote.ProxyClient.ClientCallback.TellMessageToServerConsole(ProxyGuid, sb.ToString());
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Shows help screen.")]
        static CommandResponse help(CommandRequest req, CommandPipeline pipe)
        {
            var helpString = RemotePlusConsole.ShowHelp(ProxyService.Commands, req.Arguments.Select(t => t.Value).ToArray());
            ProxyService.Remote.ProxyClient.ClientCallback.TellMessageToServerConsole(ProxyGuid, helpString);
            ProxyService.Remote.ProxyClient.ClientCallback.TellMessageToServerConsole(ProxyGuid, "\nAny commands not listed on this list will be executed on the selected server.\n");
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Establish registeration with the selected server.")]
        static CommandResponse register(CommandRequest req, CommandPipeline pipe)
        {
            ProxyService.Remote.Register(new RegisterationObject());
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
            Logger.AddOutput("Opening proxy server.", OutputLevel.Info);
            ProxyService = ProbeService<ProxyServerRemoteImpl>.CreateProxyService(typeof(IProxyServerRemote), new ProxyServerRemoteImpl(),
                9001,
                "Proxy",
                "ProxyClient",
                (m, o) => Logger.AddOutput(m, o), null);
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
            Logger.AddOutput("The proxy server state has been transferred to the faulted state.", OutputLevel.Error);
        }

        private static void ProxyService_HostClosed(object sender, EventArgs e)
        {
            Logger.AddOutput("Proxy server closed.", OutputLevel.Info);
        }

        private static void ProxyService_HostOpened(object sender, EventArgs e)
        {
            Logger.AddOutput($"Proxy server opened on port 9001", OutputLevel.Info);
        }
        public static CommandResponse Execute(CommandRequest c, CommandExecutionMode commandMode, CommandPipeline pipe)
        {
            bool throwFlag = false;
            StatusCodeDeliveryMethod scdm = StatusCodeDeliveryMethod.DoNotDeliver;
            try
            {
                Logger.AddOutput($"Executing server command {c.Arguments[0]}", OutputLevel.Info);
                try
                {
                    var command = ProxyService.Commands[c.Arguments[0].Value];
                    var ba = RemotePlusConsole.GetCommandBehavior(command);
                    if (ba != null)
                    {
                        if (ba.TopChainCommand && pipe.Count > 0)
                        {
                            Logger.AddOutput($"This is a top-level command.", OutputLevel.Error);
                            ProxyService.Remote.ProxyClient.ClientCallback.TellMessage(ProxyGuid, $"This is a top-level command.", OutputLevel.Error);
                            return new CommandResponse((int)CommandStatus.AccessDenied);
                        }
                        if (commandMode != ba.ExecutionType)
                        {
                            Logger.AddOutput($"The command requires you to be in {ba.ExecutionType} mode.", OutputLevel.Error);
                            ProxyService.Remote.ProxyClient.ClientCallback.TellMessage(ProxyGuid, $"The command requires you to be in {ba.ExecutionType} mode.", OutputLevel.Error);
                            return new CommandResponse((int)CommandStatus.AccessDenied);
                        }
                        if (ba.SupportClients != ClientSupportedTypes.Both && ((ProxyService.Remote.ProxyClient.ClientType == ClientType.GUI && ba.SupportClients != ClientSupportedTypes.GUI) || (ProxyService.Remote.ProxyClient.ClientType == ClientType.CommandLine && ba.SupportClients != ClientSupportedTypes.CommandLine)))
                        {
                            if (string.IsNullOrEmpty(ba.ClientRejectionMessage))
                            {
                                Logger.AddOutput($"Your client must be a {ba.SupportClients.ToString()} client.", OutputLevel.Error);
                                ProxyService.Remote.ProxyClient.ClientCallback.TellMessage(ProxyGuid, $"Your client must be a {ba.SupportClients.ToString()} client.", OutputLevel.Error);
                                return new CommandResponse((int)CommandStatus.UnsupportedClient);
                            }
                            else
                            {
                                Logger.AddOutput(ba.ClientRejectionMessage, OutputLevel.Error);
                                ProxyService.Remote.ProxyClient.ClientCallback.TellMessage(ProxyGuid, ba.ClientRejectionMessage, OutputLevel.Error);
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
                    Logger.AddOutput("Found command, and executing.", OutputLevel.Debug);
                    var sc = command(c, pipe);
                    if (scdm == StatusCodeDeliveryMethod.TellMessage)
                    {
                        ProxyService.Remote.ProxyClient.ClientCallback.TellMessage(ProxyGuid, $"Command {c.Arguments[0]} finished with status code {sc.ToString()}", OutputLevel.Info);
                    }
                    else if (scdm == StatusCodeDeliveryMethod.TellMessageToServerConsole)
                    {
                        ProxyService.Remote.ProxyClient.ClientCallback.TellMessageToServerConsole(ProxyGuid, new UILogItem(OutputLevel.Info, $"Command {c.Arguments[0]} finished with status code {sc.ToString()}"));
                    }
                    return sc;
                }
                catch (KeyNotFoundException)
                {
                    Logger.AddOutput($"Failed to find the command. Passing command to selected server [{ProxyService.Remote.SelectedClient.UniqueID}]", OutputLevel.Debug);
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
                    Logger.AddOutput("command failed: " + ex.Message, OutputLevel.Info);
                    ProxyService.Remote.ProxyClient.ClientCallback.TellMessageToServerConsole(ProxyGuid, new UILogItem(OutputLevel.Error, "Error whie executing command: " + ex.Message, "Server Host"));
                    return new CommandResponse((int)CommandStatus.Fail);
                }
            }
        }
    }
}