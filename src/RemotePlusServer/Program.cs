using System;
using System.ServiceModel;
using RemotePlusLibrary;
using System.IO;
using BetterLogger;
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
        public static RemoteImpl _remote = null;
        [STAThread]
        static void Main(string[] args)
        {
            //try
            //{
                var a = Assembly.GetExecutingAssembly().GetName();
                Console.WriteLine($"Welcome to {a.Name}, version: {a.Version.ToString()}\n\n");
                LoadServerCoreExtension();
                ServerManager.Logger.Log("Starting stop watch.", LogLevel.Debug);
                ServerManager.Logger.Log("NOTE: Tracing may be enabled on the server.", LogLevel.Info);
                sw = new Stopwatch();
                sw.Start();
                ServerManager.Logger.Log("Starting server core to setup server.", LogLevel.Info);
                if (CheckPrerequisites())
                {
                    bool autoStart = false;
                    if (args.Length == 1)
                    {
                        autoStart = true;
                    }
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new ServerControls(autoStart));
                }
            //}
//            catch (Exception ex)
//            {
//                if (Debugger.IsAttached)
//                {
//                    //throw;
//                }
//#if !COGNITO
//                ServerManager.Logger.Log("Internal server error: " + ex.Message, LogLevel.Error);
//                Console.Write("Press any key to exit.");
//                Console.ReadKey();
//#else
//                MessageBox.Show("Internal server error: " + ex.Message);
//#endif

//            }
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
                        core.AddServices(new ServiceCollection());
                        ServerBuilder sb = new ServerBuilder();
                        core.InitializeServer(sb);
                        sb.Run();
                        break;
                    }
                }
            }
            if(foundCore == false)
            {
                ServerManager.Logger.Log("A server core is not present. Cannot start server.", LogLevel.Error);
                Environment.Exit(-1);
            }
        }

        private static void ProxyService_HostFaulted(object sender, EventArgs e)
        {
            ServerManager.Logger.Log("The proxy server state has been transferred to the faulted state.", LogLevel.Error);
        }

        private static void ProxyService_HostClosed(object sender, EventArgs e)
        {
            ServerManager.Logger.Log("Proxy server closed.", LogLevel.Info);
        }

        private static void ProxyService_HostOpened(object sender, EventArgs e)
        {
            ServerManager.Logger.Log($"Proxy server opened on port {ServerManager.DefaultSettings.DiscoverySettings.Setup.DiscoveryPort}", LogLevel.Info);
        }

        static bool CheckPrerequisites()
        {
            ServerManager.Logger.Log("Checking prerequisites.", LogLevel.Info);
            //Check for prerequisites
            ServerPrerequisites.CheckPrivilleges();
            ServerPrerequisites.CheckNetworkInterfaces();
            ServerPrerequisites.CheckSettings();
            ServerManager.Logger.Log("Stopping stop watch.", LogLevel.Debug);
            sw.Stop();
            // Check results
            if(ServerManager.Logger.ErrorCount >= 1 && ServerManager.Logger.WarningCount == 0)
            {
                ServerManager.Logger.Log($"Unable to start server. ({ServerManager.Logger.ErrorCount} errors) Elapsed time: {sw.Elapsed.ToString()}", LogLevel.Error);
                return false;
            }
            else if(ServerManager.Logger.ErrorCount >= 1 && ServerManager.Logger.WarningCount >= 1)
            {
                ServerManager.Logger.Log($"Unable to start server. ({ServerManager.Logger.ErrorCount} errors, {ServerManager.Logger.WarningCount} warnings) Elapsed time: {sw.Elapsed.ToString()}", LogLevel.Error);
                return false;
            }
            else if(ServerManager.Logger.ErrorCount == 0 && ServerManager.Logger.WarningCount >= 1)
            {
                ServerManager.Logger.Log($"The server can start, but with warnings. ({ServerManager.Logger.WarningCount} warnings) Elapsed time: {sw.Elapsed.ToString()}", LogLevel.Warning);
                return true;
            }
            else
            {
                ServerManager.Logger.Log($"Validation passed. Elapsed time: {sw.Elapsed.ToString()}", LogLevel.Info);
                return true;
            }
        }
        public static DuplexChannelFactory<IProxyServerRemote> proxyChannelFactory = null;
        public static IProxyServerRemote proxyChannel = null;
        public static void RunInServerMode()
        {
            if (ServerManager.DefaultSettings.DiscoverySettings.DiscoveryBehavior == ProxyConnectionMode.Connect)
            {
                ServerManager.Logger.Log("The server will be part of a proxy cluster. Please use the proxy server to connect to this server.", LogLevel.Info);
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
        public static void Close()
        {
            ServerManager.ServerRemoteService.Host.Close();
            ServerManager.FileTransferService.Close();
            if(ServerManager.DefaultSettings.DiscoverySettings.DiscoveryBehavior == ProxyConnectionMode.Connect && proxyChannelFactory != null)
            {
                proxyChannel.Leave(ServerGuid);
                proxyChannelFactory.Close();
            }
            Environment.Exit(0);
        }
    }
}