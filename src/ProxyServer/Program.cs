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
using RemotePlusLibrary.Core.NodeStartup;

namespace ProxyServer
{
    public class ProxyManager : IApplication
    {
        public Guid EnvironmentGuid { get; set; } = Guid.NewGuid();
        public static ResourceStore ResourceStore;
        public static Guid ProxyGuid => GlobalServices.RunningApplication.EnvironmentGuid;
        public static IServiceManager DefaultServiceManager => IOCContainer.GetService<IServiceManager>();
        public static IRemotePlusService<ProxyServerRemoteImpl> ProxyService => DefaultServiceManager.GetService<ProxyServerRemoteImpl>();
        public static IExtensionLibraryLoader DefaultExtensionLoader => IOCContainer.GetService<IExtensionLibraryLoader>();

        public NetworkSide ExecutingSide => NetworkSide.Proxy;

        public EnvironmentState State { get; private set; } = EnvironmentState.Created;

        [STAThread]
        static void Main(string[] args)
        {
            IOCContainer.Provider.AddSingleton<IApplication>(new ProxyManager());
            ResourceStore = ResourceStore.New();
            GlobalServices.RunningApplication.Start(args).GetAwaiter().GetResult();
        }

        public Task Start(string[] args)
        {
            var a = Assembly.GetExecutingAssembly().GetName();
            Console.WriteLine($"Welcome to {a.Name}, version: {a.Version.ToString()}\n\n");
            var core = NodeCoreLoader.LoadByScan<IServerTaskBuilder>("ProxyServer.DefaultProxyServerCore",
                NetworkSide.Proxy);
            core.AddServices(IOCContainer.Provider);
            ServerTaskBuilder sb = new ServerTaskBuilder();
            core.InitializeNode(sb);
            sb.Build().RunTasks();
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

        private void RunPostServerCoreInitialization(INodeCoreStartup<IServerTaskBuilder> core)
        {
            ServerTaskBuilder sb = new ServerTaskBuilder();
            core.PostInitializeNode(sb);
            var postServerInit = sb.Build();
            postServerInit.RunTasks();
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