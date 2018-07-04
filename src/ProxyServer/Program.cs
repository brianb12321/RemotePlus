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
using System.IO;
using RemotePlusLibrary.IOC;
using Ninject;
using RemotePlusLibrary.Scripting;

namespace ProxyServer
{
    public static class ProxyManager
    {
        public static Guid ProxyGuid { get; } = Guid.NewGuid();
        public static IRemotePlusService<ProxyServerRemoteImpl> ProxyService => IOCContainer.Provider.Get<IRemotePlusService<ProxyServerRemoteImpl>>();
        public static ScriptBuilder ScriptBuilder => IOCContainer.Provider.Get<ScriptBuilder>();
        [STAThread]
        static void Main(string[] args)
        {
            var a = Assembly.GetExecutingAssembly().GetName();
            Console.WriteLine($"Welcome to {a.Name}, version: {a.Version.ToString()}\n\n");
            InitializeServerCore();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //BUG: if no form is injected, it will display a blank screen to the user.
            Form f = IOCContainer.Provider.Get<Form>();
            Application.Run(f);
        }

        private static void InitializeServerCore()
        {
            bool foundCore = false;
            foreach (string coreFile in Directory.GetFiles(Environment.CurrentDirectory))
            {
                if (Path.GetExtension(coreFile) == ".dll")
                {
                    var core = ServerCoreLoader.LoadServerCoreLibrary(coreFile);
                    if (core != null)
                    {
                        foundCore = true;
                        core.AddServices(new ServiceCollection());
                        ServerBuilder sb = new ServerBuilder();
                        core.InitializeServer(sb);
                        var serverInit = sb.Build();
                        serverInit.RunTasks();
                        break;
                    }
                }
            }
            if (foundCore == false)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("FATAL ERROR: A server core is not present. Cannot start server.");
                Console.ResetColor();
                Environment.Exit(-1);
            }
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
        public static IServerBuilder AddDefaultProxyCommands(this IServerBuilder builder)
        {
            return builder.AddTask(InitializeCommands);
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
            
        }

        internal static void Close()
        {
            ProxyService.Close();
            Environment.Exit(0);
        }
        public static CommandResponse Execute(CommandRequest c, CommandExecutionMode commandMode, CommandPipeline pipe)
        {
            bool throwFlag = false;
            StatusCodeDeliveryMethod scdm = StatusCodeDeliveryMethod.DoNotDeliver;
            try
            {
                GlobalServices.Logger.Log($"Executing server command {c.Arguments[0]}", LogLevel.Info);
                try
                {
                    var command = ProxyService.Commands[c.Arguments[0].Value];
                    var ba = RemotePlusConsole.GetCommandBehavior(command);
                    if (ba != null)
                    {
                        if (ba.TopChainCommand && pipe.Count > 0)
                        {
                            GlobalServices.Logger.Log($"This is a top-level command.", LogLevel.Error);
                            ProxyService.RemoteInterface.ProxyClient.ClientCallback.TellMessage(ProxyGuid, $"This is a top-level command.", LogLevel.Error);
                            return new CommandResponse((int)CommandStatus.AccessDenied);
                        }
                        if (commandMode != ba.ExecutionType)
                        {
                            GlobalServices.Logger.Log($"The command requires you to be in {ba.ExecutionType} mode.", LogLevel.Error);
                            ProxyService.RemoteInterface.ProxyClient.ClientCallback.TellMessage(ProxyGuid, $"The command requires you to be in {ba.ExecutionType} mode.", LogLevel.Error);
                            return new CommandResponse((int)CommandStatus.AccessDenied);
                        }
                        if (ba.SupportClients != ClientSupportedTypes.Both && ((ProxyService.RemoteInterface.ProxyClient.ClientType == ClientType.GUI && ba.SupportClients != ClientSupportedTypes.GUI) || (ProxyService.RemoteInterface.ProxyClient.ClientType == ClientType.CommandLine && ba.SupportClients != ClientSupportedTypes.CommandLine)))
                        {
                            if (string.IsNullOrEmpty(ba.ClientRejectionMessage))
                            {
                                GlobalServices.Logger.Log($"Your client must be a {ba.SupportClients.ToString()} client.", LogLevel.Error);
                                ProxyService.RemoteInterface.ProxyClient.ClientCallback.TellMessage(ProxyGuid, $"Your client must be a {ba.SupportClients.ToString()} client.", LogLevel.Error);
                                return new CommandResponse((int)CommandStatus.UnsupportedClient);
                            }
                            else
                            {
                                GlobalServices.Logger.Log(ba.ClientRejectionMessage, LogLevel.Error);
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
                    GlobalServices.Logger.Log("Found command, and executing.", LogLevel.Debug);
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
                    GlobalServices.Logger.Log($"Failed to find the command. Passing command to selected server [{ProxyService.RemoteInterface.SelectedClient.UniqueID}]", LogLevel.Debug);
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
                    GlobalServices.Logger.Log("command failed: " + ex.Message, LogLevel.Info);
                    ProxyService.RemoteInterface.ProxyClient.ClientCallback.TellMessageToServerConsole(ProxyGuid, "Error whie executing command: " + ex.Message, LogLevel.Error);
                    return new CommandResponse((int)CommandStatus.Fail);
                }
            }
        }
    }
}