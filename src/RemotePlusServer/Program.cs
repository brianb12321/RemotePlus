using System;
using System.ServiceModel;
using System.IO;
using BetterLogger;
using RemotePlusLibrary.Core;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using RemotePlusLibrary.Discovery;
using RemotePlusLibrary.Configuration.ServerSettings;
using RemotePlusServer.Core;
using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.Core.EventSystem;
using RemotePlusLibrary.Extension;
using System.Threading.Tasks;
using RemotePlusLibrary.Core.NodeStartup;

 namespace RemotePlusServer
{
    /// <summary>
    /// The class that starts the server.
    /// </summary>
    public class ServerStartup : IApplication
    {
        static Stopwatch sw = new Stopwatch();
        public NetworkSide ExecutingSide => NetworkSide.Server;
        public Guid EnvironmentGuid { get; set; } = Guid.NewGuid();

        public EnvironmentState State { get; private set; } = EnvironmentState.Created;

        [STAThread]
        static void Main(string[] args)
        {
            IOCContainer.Provider.AddSingleton<IApplication>(new ServerStartup());
            GlobalServices.RunningApplication.Start(args).GetAwaiter().GetResult();
        }

        public Task Start(string[] args)
        {
#if !SERVICE
            try
            {
                var a = Assembly.GetExecutingAssembly().GetName();
                Console.WriteLine($"Welcome to {a.Name}, version: {a.Version.ToString()}\n\n");
                sw = new Stopwatch();
                sw.Start();
                Console.WriteLine("Starting server core to setup and initialize services.");
                var core = NodeCoreLoader.LoadByScan<IServerTaskBuilder>("RemotePlusServer.DefaultServerCore", NetworkSide.Server);
                core.AddServices(IOCContainer.Provider);
                ServerTaskBuilder builder = new ServerTaskBuilder();
                core.InitializeNode(builder);
                var initializer = builder.Build();
                initializer.RunTasks();
                ServerManager.ServerRemoteService.RemoteInterface = new ServerRemoteInterface();
                if (ServerManager.DefaultSettings.DiscoverySettings.DiscoveryBehavior == ProxyConnectionMode.Connect)
                {
                    var clientCore = NodeCoreLoader.LoadByScan<IClientBuilder>("RemotePlusServer.DefaultServerCore",
                            NetworkSide.Server);
                    SetupProxyClient(clientCore);
                }
                else
                {
                    IOCContainer.Provider.AddSingleton<IEventBus, EventBus>();
                }
                RunPostServerInitialization(core);
                GlobalServices.Logger.Log("Running post init on all extensions.", LogLevel.Info);
                ServerManager.DefaultExtensionLibraryLoader.RunPostInit();
                if (CheckPrerequisites())
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    //BUG: if no form is injected, it will display a blank screen to the user.
                    Form serverControl = IOCContainer.GetService<Form>();
                    State = EnvironmentState.Running;
                    Application.Run(serverControl);
                }
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    //throw;
                }
#if !INCOGNITO
                GlobalServices.Logger.Log("Internal server error: " + ex, LogLevel.Error);
                Console.Write("Press any key to exit.");
                Console.ReadKey();
#else
                MessageBox.Show("Internal server error: " + ex.Message);
#endif
                return Task.FromException(ex);
            }
#else
            ServerManager.IsService = true;
            ServiceBase.Run(new RemotePlusWindowsService());
            //new RemotePlusWindowsService().StartforDebugging();
#endif
        }

        private void RunPostServerInitialization(INodeCoreStartup<IServerTaskBuilder> core)
        {
            ServerTaskBuilder sb = new ServerTaskBuilder();
            core.PostInitializeNode(sb);
            var postServerInit = sb.Build();
            postServerInit.RunTasks();
        }

        static bool CheckPrerequisites()
        {
            GlobalServices.Logger.Log("Checking prerequisites.", LogLevel.Info);
            //Check for prerequisites
            ServerPrerequisites.CheckPrivilleges();
            ServerPrerequisites.CheckNetworkInterfaces();
            ServerPrerequisites.CheckSettings();
            GlobalServices.Logger.Log("Stopping stop watch.", LogLevel.Debug);
            sw.Stop();
            // Check results
            if(GlobalServices.Logger.ErrorCount >= 1 && GlobalServices.Logger.WarningCount == 0)
            {
                GlobalServices.Logger.Log($"Unable to start server. ({GlobalServices.Logger.ErrorCount} errors) Elapsed time: {sw.Elapsed.ToString()}", LogLevel.Error);
                return false;
            }
            else if(GlobalServices.Logger.ErrorCount >= 1 && GlobalServices.Logger.WarningCount >= 1)
            {
                GlobalServices.Logger.Log($"Unable to start server. ({GlobalServices.Logger.ErrorCount} errors, {GlobalServices.Logger.WarningCount} warnings) Elapsed time: {sw.Elapsed.ToString()}", LogLevel.Error);
                return false;
            }
            else if(GlobalServices.Logger.ErrorCount == 0 && GlobalServices.Logger.WarningCount >= 1)
            {
                GlobalServices.Logger.Log($"The server can start, but with warnings. ({GlobalServices.Logger.WarningCount} warnings) Elapsed time: {sw.Elapsed.ToString()}", LogLevel.Warning);
                return true;
            }
            else
            {
                GlobalServices.Logger.Log($"Validation passed. Elapsed time: {sw.Elapsed.ToString()}", LogLevel.Info);
                return true;
            }
        }
        public static DuplexChannelFactory<IProxyServerRemote> proxyChannelFactory = null;
        public static IProxyServerRemote proxyChannel = null;

        public ServerStartup()
        {
        }
        public static void SetupProxyClient(INodeCoreStartup<IClientBuilder> core)
        {
            proxyChannelFactory = new DuplexChannelFactory<IProxyServerRemote>(new RemoteImpl(), _ConnectionFactory.BuildBinding(), new EndpointAddress(ServerManager.DefaultSettings.DiscoverySettings.Connection.ProxyServerURL));
            IOCContainer.Provider.AddSingleton(proxyChannelFactory);
            proxyChannel = proxyChannelFactory.CreateChannel();
            IOCContainer.Provider.AddSingleton(proxyChannel);
            IOCContainer.Provider.AddSingleton<IEventBus, ProxyEventBus>();
        }
        public static void RunInServerMode()
        {
            if (ServerManager.DefaultSettings.DiscoverySettings.DiscoveryBehavior == ProxyConnectionMode.Connect)
            {
                GlobalServices.Logger.Log("The server will be part of a proxy cluster. Please use the proxy server to connect to this server.", LogLevel.Info);
                proxyChannel.Register();
            }
            else
            {
                ServerManager.ServerRemoteService.Start();
                ServerManager.FileTransferService.Start();
            }
        }
        public void Close()
        {
            if (ServerManager.DefaultSettings.DiscoverySettings.DiscoveryBehavior == ProxyConnectionMode.Connect && proxyChannelFactory != null)
            {
                proxyChannel.Leave(ServerManager.ServerGuid);
                proxyChannelFactory.Close();
            }
            ServerManager.DefaultServiceManager.CloseAll();
            Environment.Exit(0);
        }
    }
}