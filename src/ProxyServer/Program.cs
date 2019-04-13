using System;
using System.Text;
using RemotePlusLibrary;
using System.Reflection;
using System.Windows.Forms;
using BetterLogger;
using System.IO;
using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.Scripting;
using RemotePlusLibrary.ServiceArchitecture;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Extension.ResourceSystem;
using ProxyServer.ExtensionSystem;
using RemotePlusLibrary.Extension;
using Ninject;
using System.Linq;
using RemotePlusLibrary.SubSystem.Command;
using System.Threading.Tasks;

namespace ProxyServer
{
    public class ProxyManager : IEnvironment
    {
        public static ResourceStore ResourceStore;
        public static Guid ProxyGuid { get; } = Guid.NewGuid();
        public static IServiceManager DefaultServiceManager => IOCContainer.GetService<IServiceManager>();
        public static IRemotePlusService<ProxyServerRemoteImpl> ProxyService => DefaultServiceManager.GetService<ProxyServerRemoteImpl>();
        public static IExtensionLibraryLoader DefaultExtensionLoader => IOCContainer.GetService<IExtensionLibraryLoader>();

        public NetworkSide ExecutingSide => NetworkSide.Proxy;

        public EnvironmentState State { get; private set; } = EnvironmentState.Created;

        [STAThread]
        static void Main(string[] args)
        {
            IOCContainer.Provider.AddSingleton<IEnvironment>(new ProxyManager());
            ResourceStore = ResourceStore.New();
            GlobalServices.RunningEnvironment.Start(args).GetAwaiter().GetResult();
        }

        public Task Start(string[] args)
        {
            var a = Assembly.GetExecutingAssembly().GetName();
            Console.WriteLine($"Welcome to {a.Name}, version: {a.Version.ToString()}\n\n");
            var core = InitializeServerCore();
            DefaultExtensionLoader.LoadFromFolder("extensions");
            DefaultExtensionLoader.LoadFromAssembly(Assembly.GetAssembly(typeof(ProxyCommands)));
            RunPostServerCoreInitialization(core);
            GlobalServices.Logger.Log("Running post init on all extensions.", LogLevel.Info);
            DefaultExtensionLoader.RunPostInit();
            IOCContainer.Provider.GetService<ICommandSubsystem<IProxyCommandModule>>().Init();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //BUG: if no form is injected, it will display a blank screen to the user.
            Form f = IOCContainer.GetService<Form>();
            State = EnvironmentState.Running;
            Application.Run(f);
            return Task.CompletedTask;
        }

        private void RunPostServerCoreInitialization(IServerCoreStartup core)
        {
            ServerBuilder sb = new ServerBuilder();
            core.PostInitializeServer(sb);
            var postServerInit = sb.Build();
            postServerInit.RunTasks();
        }

        private IServerCoreStartup InitializeServerCore()
        {
            bool foundCore = false;
            foreach (string coreFile in Directory.GetFiles(Environment.CurrentDirectory))
            {
                if (Path.GetExtension(coreFile) == ".dll")
                {
                    var core = ServerCoreLoader.LoadServerCore(Assembly.LoadFile(coreFile));
                    if (core != null)
                    {
                        foundCore = true;
                        core.AddServices(IOCContainer.Provider);
                        ServerBuilder sb = new ServerBuilder();
                        core.InitializeServer(sb);
                        var serverInit = sb.Build();
                        serverInit.RunTasks();
                        return core;
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
                return null;
            }
            return null;
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