using System;
using BetterLogger.Loggers;
using RemotePlusLibrary.Core.IOC;
using ProxyServer;
using BetterLogger;
using RemotePlusLibrary.Discovery;
using RemotePlusLibrary;
using RemotePlusLibrary.ServiceArchitecture;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses.Parsing;
using RemotePlusLibrary.Core.EventSystem;
using RemotePlusLibrary.Core;

namespace ProxyServerCore
{
    public class Startup : IServerCoreStartup
    {
        public void AddServices(IServiceCollection services)
        {
            services.UseLogger((logFactory) => logFactory.AddLogger(new ConsoleLogger()))
                .UseServerManager<DefaultServiceManager>()
                .UseScriptingEngine()
                .UseEventBus<EventBus>()
                .UseServerControlPage<ServerControls>()
                .UseConfigurationDataAccess<RemotePlusLibrary.Configuration.StandordDataAccess.ConfigurationHelper>()
                .UseCommandline<CommandEnvironment>(builder =>
                    builder.UseParser<CommandParser>()
                   .UseProcessor<TokenProcessor>()
                   .UseExecutor<CommandExecutor>());

            //Set up WCF services.
            IServiceManager manager = IOCContainer.GetService<IServiceManager>();
            manager.AddServiceUsingBuilder<ProxyServerRemoteImpl>(() =>
            {
                ProbeServiceBuilder psb = new ProbeServiceBuilder(new ProxyServerRemoteImpl(), _ConnectionFactory.BuildBinding());
                return psb.SetBinding(_ConnectionFactory.BuildBinding())
                    .SetPortNumber(8080)
                    .RouteHostClosedEvent(ProxyService_HostClosed)
                    .RouteHostClosingEvent(ProxyService_HostClosing)
                    .RouteHostFaultedEvent(ProxyService_HostFaulted)
                    .RouteHostOpenEvent(ProxyService_HostOpened)
                    .RouteHostOpeningEvent(ProxyService_HostOpening)
                    .RouteUnknownMessageReceivedEvent(ProxyService_UnknownMessageReceived);
            });
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
        private static void ProxyService_HostClosing(object sender, EventArgs e)
        {
            GlobalServices.Logger.Log("Closing proxy server.", LogLevel.Info);
        }

        private static void ProxyService_HostOpened(object sender, EventArgs e)
        {
            GlobalServices.Logger.Log($"Proxy server opened on port 8080.", LogLevel.Info);
        }
        private static void ProxyService_UnknownMessageReceived(object sender, System.ServiceModel.UnknownMessageReceivedEventArgs e)
        {
            GlobalServices.Logger.Log($"An unknown message has been received. Message: {e.Message}", LogLevel.Info);
        }
        private static void ProxyService_HostOpening(object sender, EventArgs e)
        {
            GlobalServices.Logger.Log($"Opening proxy server.", LogLevel.Info);
        }
    }
}