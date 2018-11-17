using System;
using System.Text;
using RemotePlusLibrary;
using System.Reflection;
using System.Windows.Forms;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using BetterLogger;
using System.IO;
using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.Scripting;
using RemotePlusLibrary.Extension.ExtensionLoader;
using RemotePlusLibrary.ServiceArchitecture;
using RemotePlusLibrary.Core;

namespace ProxyServer
{
    public class ProxyManager : IEnvironment
    {
        public static Guid ProxyGuid { get; } = Guid.NewGuid();
        public static IServiceManager DefaultServiceManager => IOCContainer.GetService<IServiceManager>();
        public static IRemotePlusService<ProxyServerRemoteImpl> ProxyService => DefaultServiceManager.GetService<ProxyServerRemoteImpl>();
        public static ScriptBuilder ScriptBuilder => IOCContainer.GetService<ScriptBuilder>();
        public static ExtensionSystem.ProxyExtensionCollection DefaultCollection { get; private set; } = new ExtensionSystem.ProxyExtensionCollection();
        public static RemotePlusLibrary.FileTransfer.Service.PackageSystem.IPackageInventorySelector DefaultPackageInventorySelector => IOCContainer.GetService<RemotePlusLibrary.FileTransfer.Service.PackageSystem.IPackageInventorySelector>();

        public NetworkSide ExecutingSide => NetworkSide.Server;

        public EnvironmentState State { get; private set; } = EnvironmentState.Created;

        [STAThread]
        static void Main(string[] args)
        {
            IOCContainer.Provider.Bind<IEnvironment>().ToConstant(new ProxyManager());
            GlobalServices.RunningEnvironment.Start(args);
        }

        public void Start(string[] args)
        {
            var a = Assembly.GetExecutingAssembly().GetName();
            Console.WriteLine($"Welcome to {a.Name}, version: {a.Version.ToString()}\n\n");
            InitializeServerCore();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //BUG: if no form is injected, it will display a blank screen to the user.
            Form f = IOCContainer.GetService<Form>();
            State = EnvironmentState.Running;
            Application.Run(f);
        }
        private void InitializeServerCore()
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
                State = EnvironmentState.Closing;
                Environment.Exit(-1);
            }
        }

        [CommandHelp("Switches the specified server into the active server.")]
        internal static CommandResponse switchServer(CommandRequest req, CommandPipeline pipe)
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
        internal static CommandResponse viewServers(CommandRequest req, CommandPipeline pipe)
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
        internal static CommandResponse help(CommandRequest req, CommandPipeline pipe)
        {
            string helpString = string.Empty;
            if (req.Arguments.Count == 2)
            {
                helpString = RemotePlusConsole.ShowHelpPage(ProxyService.Commands, req.Arguments[1].Value);
            }
            else
            {
                helpString = RemotePlusConsole.ShowHelp(ProxyService.Commands);
            }
            ProxyService.RemoteInterface.ProxyClient.ClientCallback.TellMessageToServerConsole(ProxyGuid, helpString);
            ProxyService.RemoteInterface.ProxyClient.ClientCallback.TellMessageToServerConsole(ProxyGuid, "\nAny commands not listed on this list will be executed on the selected server.\n");
            var response = new CommandResponse((int)CommandStatus.Success);
            response.Metadata.Add("helpText", helpString);
            return response;
        }
        [CommandHelp("Establish registration with the selected server.")]
        internal static CommandResponse register(CommandRequest req, CommandPipeline pipe)
        {
            ProxyService.RemoteInterface.Register(new RegisterationObject());
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Clears all variables and functions from the interactive scripts.")]
        internal static CommandResponse resetStaticScript(CommandRequest reqest, CommandPipeline pipe)
        {
            ProxyManager.ScriptBuilder.ClearStaticScope();
            return new CommandResponse((int)CommandStatus.Success);
        }

        

        internal static void RunInServerMode()
        {
            ProxyService.Start();
        }

        public void Close()
        {
            State = EnvironmentState.Closing;
            DefaultServiceManager.CloseAll();
            Environment.Exit(0);
        }
    }
}