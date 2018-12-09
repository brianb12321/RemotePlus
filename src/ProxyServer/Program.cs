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
using RemotePlusLibrary.Extension.ResourceSystem;

namespace ProxyServer
{
    public class ProxyManager : IEnvironment
    {
        public static ResourceStore ResourceStore = ResourceStore.New();
        public static Guid ProxyGuid { get; } = Guid.NewGuid();
        public static IServiceManager DefaultServiceManager => IOCContainer.GetService<IServiceManager>();
        public static IRemotePlusService<ProxyServerRemoteImpl> ProxyService => DefaultServiceManager.GetService<ProxyServerRemoteImpl>();
        public static ScriptBuilder ScriptBuilder => IOCContainer.GetService<ScriptBuilder>();
        public static ExtensionLibraryCollectionBase<ProxyServer.ExtensionSystem.ProxyExtensionLibrary> DefaultCollection => IOCContainer.GetService<ExtensionLibraryCollectionBase<ExtensionSystem.ProxyExtensionLibrary>>();
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