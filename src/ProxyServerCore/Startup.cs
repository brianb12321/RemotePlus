using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BetterLogger.Loggers;
using RemotePlusLibrary.Core.IOC;
using ProxyServer;
using BetterLogger;
using RemotePlusLibrary.Discovery;
using RemotePlusLibrary;

namespace ProxyServerCore
{
    public class Startup : IServerCoreStartup
    {
        public void AddServices(IServiceCollection services)
        {
            services.UseLogger((logFactory) => logFactory.AddLogger(new ConsoleLogger()))
                .AddServer(() =>
                {
                    GlobalServices.Logger.Log("Opening proxy server.", LogLevel.Info);
                    var proxyService = ProbeService.CreateProxyService(typeof(IProxyServerRemote), new ProxyServerRemoteImpl(),
                        8080,
                        "Proxy",
                        "ProxyClient",
                        (m, o) => GlobalServices.Logger.Log(m, o), null);
                    proxyService.HostOpened += ProxyService_HostOpened;
                    proxyService.HostClosed += ProxyService_HostClosed;
                    proxyService.HostFaulted += ProxyService_HostFaulted;
                    return proxyService;
                })
                .UseScriptingEngine()
                .UseServerControlPage<ServerControls>()
                .UseConfigurationDataAccess<RemotePlusLibrary.Configuration.StandordDataAccess.ConfigurationHelper>();
        }
        public void InitializeServer(IServerBuilder builder)
        {
            builder.AddDefaultProxyCommands()
                .InitializeGlobals()
                .InitializeScriptingEngine();
        }
        private static void ProxyService_HostFaulted(object sender, EventArgs e)
        {
            GlobalServices.Logger.Log("The proxy server state has been transferred to the faulted state.", LogLevel.Error);
        }

        private static void ProxyService_HostClosed(object sender, EventArgs e)
        {
            GlobalServices.Logger.Log("Proxy server closed.", LogLevel.Info);
        }

        private static void ProxyService_HostOpened(object sender, EventArgs e)
        {
            GlobalServices.Logger.Log($"Proxy server opened on port 9001", LogLevel.Info);
        }
    }
}