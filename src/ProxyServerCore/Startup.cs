using System;
using BetterLogger.Loggers;
using RemotePlusLibrary.Core.IOC;
using ProxyServer;
using ProxyServer.ExtensionSystem;
using BetterLogger;
using RemotePlusLibrary;
using RemotePlusLibrary.Configuration;
using RemotePlusLibrary.ServiceArchitecture;
using RemotePlusLibrary.Core.EventSystem;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Core.Behavior;
using RemotePlusLibrary.Core.NodeStartup;
using RemotePlusLibrary.Discovery;
using RemotePlusLibrary.Extension.ResourceSystem;
using RemotePlusLibrary.Scripting;
using RemotePlusLibrary.Extension;
using RemotePlusLibrary.SubSystem.Command;
using RemotePlusLibrary.SubSystem.Command.CommandClasses;
using RemotePlusLibrary.SubSystem.Command.CommandClasses.Parsing;

namespace ProxyServerCore
{
    public class Startup : IServerCoreStartup
    {
        public void AddServices(IServiceCollection services)
        {
            services.UseLogger((logFactory) => logFactory.AddLogger(new ConsoleLogger()));
            services.UseServerManager<DefaultServiceManager>()
                .AddSingleton<IEventBus, EventBus>()
                .AddSingleton<IServerListManager, DefaultServerListManager>()
                .UseResourceManager<ProxyResourceManager, FileResourceLoader>()
                .UseErrorHandler<GlobalErrorHandler>()
                .UseExtensionSystem<DefaultExtensionLoader>()
                .UseScriptingEngine<IronPythonScriptingEngine>()
                .UseServerControlPage<ServerControls>()
                .UseConfigurationDataAccess<RemotePlusLibrary.Configuration.StandordDataAccess.ConfigurationHelper>()
                .UseCommandline<CommandEnvironment, ProxyCommandSubsystem, IProxyCommandModule>(builder =>
                    builder.UseLexer<CommandLexer>()
                   .UseParser<CommandParser>()
                   .UseExecutor<CommandExecutor>());

            //Set up WCF services.
            IServiceManager manager = IOCContainer.GetService<IServiceManager>();
            manager.AddServiceUsingBuilder(() =>
            {
                ProbeServiceBuilder psb = new ProbeServiceBuilder(new ProxyServerRemoteImpl(), _ConnectionFactory.BuildBinding());
                return psb.SetBinding(_ConnectionFactory.BuildBinding())
                    .SetPortNumber(8080)
                    .RouteHostClosedEvent(ProxyService_HostClosed)
                    .RouteHostClosingEvent(ProxyService_HostClosing)
                    .RouteHostFaultedEvent(ProxyService_HostFaulted)
                    .RouteHostOpenEvent(ProxyService_HostOpened)
                    .RouteHostOpeningEvent(ProxyService_HostOpening)
                    .RouteUnknownMessageReceivedEvent(ProxyService_UnknownMessageReceived)
                    .AddServiceBehavior(new GlobalExceptionBehavior());
            });
        }

        public void InitializeNode(IServerTaskBuilder builder)
        {
            builder.InitializeKnownTypes();
            builder.InitializeGlobals()
                .LoadGlobalResources();
        }

        public void PostInitializeNode(IServerTaskBuilder builder)
        {
            builder.BuildServiceHost<IServerTaskBuilder, ProxyServerRemoteImpl>();
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