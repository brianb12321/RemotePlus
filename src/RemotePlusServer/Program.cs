using System;
using System.ServiceModel;
using RemotePlusLibrary;
using System.IO;
using Logging;
using RemotePlusLibrary.Core;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using RemotePlusLibrary.FileTransfer.Service;
using System.ServiceModel.Description;
using RemotePlusLibrary.Discovery;
using RemotePlusLibrary.Configuration.ServerSettings;
using RemotePlusLibrary.Contracts;
using RemotePlusServer.Core;
using RemotePlusServer.Core.ServerCore;

namespace RemotePlusServer
{
    /// <summary>
    /// The class that starts the server.
    /// </summary>
    public static partial class ServerStartup
    {
        static Stopwatch sw = new Stopwatch();
        static Guid ServerGuid = Guid.NewGuid();
        private static RemoteImpl _remote = null;
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                var a = Assembly.GetExecutingAssembly().GetName();
                Console.WriteLine($"Welcome to {a.Name}, version: {a.Version.ToString()}\n\n");
                IOCContainer.Setup();
                ServerManager.Logger.DefaultFrom = "Server Host";
                ServerManager.Logger.AddOutput("Starting stop watch.", OutputLevel.Debug);
                ServerManager.Logger.AddOutput(new LogItem(OutputLevel.Info, "NOTE: Tracing may be enabled on the server.", "Server Host") { Color = ConsoleColor.Cyan });
                sw = new Stopwatch();
                sw.Start();
                ServerManager.Logger.AddOutput("Starting server core to setup server.", OutputLevel.Info);
                LoadServerCoreExtension();
                if (CheckPrerequisites())
                {
                    bool autoStart = false;
                    if(args.Length == 1)
                    {
                        autoStart = true;
                    }
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new ServerControls(autoStart));
                }
        }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    //throw;
                }
#if !COGNITO
    ServerManager.Logger.AddOutput("Internal server error: " + ex.Message, OutputLevel.Error);
                Console.Write("Press any key to exit.");
                Console.ReadKey();
                SaveLog();
#else
                MessageBox.Show("Internal server error: " + ex.Message);
                SaveLog();
#endif

}
        }

        private static void LoadServerCoreExtension()
        {
            bool foundCore = false;
            foreach(string coreFile in Directory.GetFiles(Environment.CurrentDirectory))
            {
                if(Path.GetExtension(coreFile) == ".dll")
                {
                    var core = ServerCoreLoader.LoadServerCoreLibrary(coreFile);
                    if(core != null)
                    {
                        foundCore = true;
                        ServerBuilder sb = new ServerBuilder();
                        core.InitializeServer(sb);
                        sb.Run();
                        break;
                    }
                }
            }
            if(foundCore == false)
            {
                ServerManager.Logger.AddOutput("A server core is not present. Cannot start server.", OutputLevel.Error);
                Environment.Exit(-1);
            }
        }

        private static void CreateServer()
        {
            var endpointAddress = "Remote";
            if(ServerManager.DefaultSettings.DiscoverySettings.DiscoveryBehavior == ProxyConnectionMode.Connect)
            {
                endpointAddress += $"/{Guid.NewGuid()}";
            }
            _remote = new RemoteImpl();
            var service = ServerRemotePlusService.Create(typeof(IRemote), _remote, ServerManager.DefaultSettings.PortNumber,endpointAddress, (m, o) => ServerManager.Logger.AddOutput(m, o), null);
            ServiceThrottlingBehavior throt = new System.ServiceModel.Description.ServiceThrottlingBehavior();
            throt.MaxConcurrentCalls = int.MaxValue;
            service.Host.Description.Behaviors.Add(throt);
            SetupFileTransferService();
            ServerManager.Logger.AddOutput("Attaching server events.", OutputLevel.Debug);
            service.HostClosed += Host_Closed;
            service.HostClosing += Host_Closing;
            service.HostFaulted += Host_Faulted;
            service.HostOpened += Host_Opened;
            service.HostOpening += Host_Opening;
            service.HostUnknownMessageReceived += Host_UnknownMessageReceived;
            OpenMex(service, ServerManager.FileTransferService);
            IOCContainer.Kernel.Bind<IRemotePlusService<ServerRemoteInterface>>().ToConstant(service);
        }

        private static void ProxyService_HostFaulted(object sender, EventArgs e)
        {
            ServerManager.Logger.AddOutput("The proxy server state has been transferred to the faulted state.", OutputLevel.Error);
        }

        private static void ProxyService_HostClosed(object sender, EventArgs e)
        {
            ServerManager.Logger.AddOutput("Proxy server closed.", OutputLevel.Info);
        }

        private static void ProxyService_HostOpened(object sender, EventArgs e)
        {
            ServerManager.Logger.AddOutput($"Proxy server opened on port {ServerManager.DefaultSettings.DiscoverySettings.Setup.DiscoveryPort}", OutputLevel.Info);
        }

        private static void OpenMex(IRemotePlusService<ServerRemoteInterface> mainService, IRemotePlusService<FileTransferServciceInterface> fileTransfer)
        {
            if(ServerManager.DefaultSettings.EnableMetadataExchange)
            {
                ServerManager.Logger.AddOutput(new LogItem(OutputLevel.Info, "NOTE: Metadata exchange is enabled on the server.", "Server Host" ) { Color = ConsoleColor.Cyan });
                System.ServiceModel.Channels.Binding mexBinding = MetadataExchangeBindings.CreateMexHttpBinding();
                ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
                smb.HttpGetEnabled = true;
                smb.HttpGetUrl = new Uri("http://0.0.0.0:9001/Mex");
                ServiceMetadataBehavior smb2 = new ServiceMetadataBehavior();
                smb.HttpGetEnabled = true;
                smb.HttpGetUrl = new Uri("http://0.0.0.0:9001/Mex2");
                mainService.Host.Description.Behaviors.Add(smb);
                fileTransfer.Host.Description.Behaviors.Add(smb2);
                mainService.Host.AddServiceEndpoint(typeof(IMetadataExchange), mexBinding, "http://0.0.0.0:9001/Mex");
                fileTransfer.Host.AddServiceEndpoint(typeof(IMetadataExchange), mexBinding, "http://0.0.0.0:9001/Mex2");
            }
        }

        private static void SetupFileTransferService()
        {
            IRemotePlusService<FileTransferServciceInterface> fts = null;
            ServerManager.Logger.AddOutput("Adding file transfer service.", OutputLevel.Info);
            var binding = _ConnectionFactory.BuildBinding();
            binding.TransferMode = TransferMode.Streamed;
            fts = FileTransferService.CreateNotSingle(typeof(IFileTransferContract), ServerManager.DefaultSettings.PortNumber, binding, "FileTransfer", null);
            fts.HostClosed += Host_Closed;
            fts.HostClosing += Host_Closing;
            fts.HostFaulted += Host_Faulted;
            fts.HostOpened += Host_Opened;
            fts.HostOpening += Host_Opening;
            fts.HostUnknownMessageReceived += Host_UnknownMessageReceived;
            IOCContainer.Kernel.Bind<IRemotePlusService<FileTransferServciceInterface>>().ToConstant(fts);
        }

        private static void SaveLog()
        {
            try
            {
                if (ServerManager.DefaultSettings.LoggingSettings.LogOnShutdown)
                {
                    ServerManager.Logger.AddOutput("Saving log and closing.", OutputLevel.Info);
                    ServerManager.Logger.SaveLog($"{ServerManager.DefaultSettings.LoggingSettings.LogFolder}\\{DateTime.Now.ToShortDateString().Replace('/', ServerManager.DefaultSettings.LoggingSettings.DateDelimiter)} {DateTime.Now.ToShortTimeString().Replace(':', ServerManager.DefaultSettings.LoggingSettings.TimeDelimiter)}.txt");
                }
            }
            catch (Exception ex)
            {
                ServerManager.Logger.AddOutput($"Unable to save log file: {ex.Message}", OutputLevel.Error);
            }
        }

        static bool CheckPrerequisites()
        {
            ServerManager.Logger.AddOutput("Checking prerequisites.", OutputLevel.Info);
            //Check for prerequisites
            ServerPrerequisites.CheckPrivilleges();
            ServerPrerequisites.CheckNetworkInterfaces();
            ServerPrerequisites.CheckSettings();
            ServerManager.Logger.AddOutput("Stopping stop watch.", OutputLevel.Debug);
            sw.Stop();
            // Check results
            if(ServerManager.Logger.errorcount >= 1 && ServerManager.Logger.warningcount == 0)
            {
                ServerManager.Logger.AddOutput($"Unable to start server. ({ServerManager.Logger.errorcount} errors) Elapsed time: {sw.Elapsed.ToString()}", OutputLevel.Error);
                return false;
            }
            else if(ServerManager.Logger.errorcount >= 1 && ServerManager.Logger.warningcount >= 1)
            {
                ServerManager.Logger.AddOutput($"Unable to start server. ({ServerManager.Logger.errorcount} errors, {ServerManager.Logger.warningcount} warnings) Elapsed time: {sw.Elapsed.ToString()}", OutputLevel.Error);
                return false;
            }
            else if(ServerManager.Logger.errorcount == 0 && ServerManager.Logger.warningcount >= 1)
            {
                ServerManager.Logger.AddOutput($"The server can start, but with warnings. ({ServerManager.Logger.warningcount} warnings) Elapsed time: {sw.Elapsed.ToString()}", OutputLevel.Warning);
                return true;
            }
            else
            {
                ServerManager.Logger.AddOutput(new LogItem(OutputLevel.Info, $"Validation passed. Elapsed time: {sw.Elapsed.ToString()}", "Server Host") { Color = ConsoleColor.Green });
                return true;
            }
        }
        public static DuplexChannelFactory<IProxyServerRemote> proxyChannelFactory = null;
        public static IProxyServerRemote proxyChannel = null;
        public static void RunInServerMode()
        {
            if (ServerManager.DefaultSettings.DiscoverySettings.DiscoveryBehavior == ProxyConnectionMode.Connect)
            {
                ServerManager.Logger.AddOutput("The server will be part of a proxy cluster. Please use the proxy server to connect to this server.", OutputLevel.Info);
                proxyChannelFactory = new DuplexChannelFactory<IProxyServerRemote>(_remote, _ConnectionFactory.BuildBinding(), new EndpointAddress(ServerManager.DefaultSettings.DiscoverySettings.Connection.ProxyServerURL));
                proxyChannel = proxyChannelFactory.CreateChannel();
                proxyChannel.Register();
            }
            else
            {
                ServerManager.ServerRemoteService.Start();
                ServerManager.FileTransferService.Start();
            }
        }

        private static void Host_UnknownMessageReceived(object sender, UnknownMessageReceivedEventArgs e)
        {
            ServerManager.Logger.AddOutput($"The server encountered an unknown message sent by the client. Message: {e.Message.ToString()}", OutputLevel.Error);
        }

        private static void Host_Opening(object sender, EventArgs e)
        {
            ServerManager.Logger.AddOutput("Opening server.", OutputLevel.Info);
        }

        private static void Host_Opened(object sender, EventArgs e)
        {
            if (ServerManager.DefaultSettings.DiscoverySettings.DiscoveryBehavior == ProxyConnectionMode.Connect)
            {
                ServerManager.Logger.AddOutput($"Host ready. Server is now part of the proxy cluster. Connect to proxy server to configure this server.", OutputLevel.Info);
            }
            else
            {
                ServerManager.Logger.AddOutput($"Host ready. Server is listening on port {ServerManager.DefaultSettings.PortNumber}. Connect to configure server.", Logging.OutputLevel.Info);
            }
        }

        private static void Host_Faulted(object sender, EventArgs e)
        {
            ServerManager.Logger.AddOutput("The server state has been transferred to the faulted state.", OutputLevel.Error);
        }

        private static void Host_Closing(object sender, EventArgs e)
        {
            ServerManager.Logger.AddOutput("Closing the server.", OutputLevel.Info);
        }

        private static void Host_Closed(object sender, EventArgs e)
        {
            ServerManager.Logger.AddOutput("The server is now closed.", OutputLevel.Info);
        }

        public static void Close()
        {
            SaveLog();
            ServerManager.ServerRemoteService.Host.Close();
            ServerManager.FileTransferService.Close();
            if(ServerManager.DefaultSettings.DiscoverySettings.DiscoveryBehavior == ProxyConnectionMode.Connect && proxyChannelFactory != null)
            {
                proxyChannel.Leave(ServerGuid);
                proxyChannelFactory.Close();
            }
            Environment.Exit(0);
        }
        public static IServerBuilder CreateServer(this IServerBuilder builder)
        {
            return builder.AddTask(() =>
            {
                var endpointAddress = "Remote";
                if (ServerManager.DefaultSettings.DiscoverySettings.DiscoveryBehavior == ProxyConnectionMode.Connect)
                {
                    endpointAddress += $"/{Guid.NewGuid()}";
                }
                _remote = new RemoteImpl();
                var service = ServerRemotePlusService.Create(typeof(IRemote), _remote, ServerManager.DefaultSettings.PortNumber, endpointAddress, (m, o) => ServerManager.Logger.AddOutput(m, o), null);
                ServiceThrottlingBehavior throt = new System.ServiceModel.Description.ServiceThrottlingBehavior();
                throt.MaxConcurrentCalls = int.MaxValue;
                service.Host.Description.Behaviors.Add(throt);
                SetupFileTransferService();
                ServerManager.Logger.AddOutput("Attaching server events.", OutputLevel.Debug);
                service.HostClosed += Host_Closed;
                service.HostClosing += Host_Closing;
                service.HostFaulted += Host_Faulted;
                service.HostOpened += Host_Opened;
                service.HostOpening += Host_Opening;
                service.HostUnknownMessageReceived += Host_UnknownMessageReceived;
                OpenMex(service, ServerManager.FileTransferService);
                IOCContainer.Kernel.Bind<IRemotePlusService<ServerRemoteInterface>>().ToConstant(service);
            });
        }
    }
}