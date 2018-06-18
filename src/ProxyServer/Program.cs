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

namespace ProxyServer
{
    class ProxyManager
    {
        public static CMDLogging Logger { get; } = new CMDLogging();
        static ProbeService<ProxyServerRemoteImpl> ProxyService { get; set; }
        [STAThread]
        static void Main(string[] args)
        {
            Logger.DefaultFrom = "Proxy Server";
            var a = Assembly.GetExecutingAssembly().GetName();
            Console.WriteLine($"Welcome to {a.Name}, version: {a.Version.ToString()}\n\n");
            CreateProxyServer();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ServerControls(false));
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
    }
}