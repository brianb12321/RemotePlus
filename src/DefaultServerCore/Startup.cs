using System;
using BetterLogger;
using RemotePlusServer.Core;
using RemotePlusServer.Core.ServerCore;
using RemotePlusServer;
using RemotePlusLibrary;
using RemotePlusLibrary.ServiceArchitecture;
using BetterLogger.Loggers;
using RemotePlusLibrary.Configuration;
using RemotePlusLibrary.Configuration.StandordDataAccess;
using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.Security.AccountSystem;
using RSPM;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses.Parsing;
using RemotePlusLibrary.Core.EventSystem;
using RemotePlusLibrary.Core;
using RemotePlusServer.Core.ExtensionSystem;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using RemotePlusLibrary.Extension.ResourceSystem;

namespace DefaultServerCore
{
    public sealed class Startup : IServerCoreStartup
    {
        public void AddServices(IServiceCollection services)
        {
            services.UseLogger(logFactory =>
            {
                logFactory.AddLogger(new ConsoleLogger()
                {
                    Settings = new ConsoleLoggerOptions()
                    {
                        VerboseColor = ConsoleColor.DarkMagenta
                    }
                });
            });
            services.UseServerManager<DefaultServiceManager>()
                .UseResourceManager<RemotePlusResourceManager, FileResourceLoader>()
                .UseErrorHandler<GlobalErrorHandler>()
                .UseExtensionContainer<ServerExtensionLibraryCollection, ServerExtensionLibrary>(new ServerExtensionLibraryCollection())
                .UseServerControlPage<ServerControls>()
                .UseScriptingEngine()
                .UseConfigurationDataAccess<ConfigurationHelper>()
                .AddSingletonNamed<IConfigurationDataAccess, BinarySerializationHelper>("BinaryDataAccess")
                .UseAuthentication<AccountManager>()
                .UsePackageManager<DefaultPackageManager>()
                .UseCommandline<CommandEnvironment>(builder =>
                    builder.UseLexer<CommandLexer>()
                           .UseParser<CommandParser>()
                           .UseExecutor<CommandExecutor>()
                           .AddCommandClass<DefaultCommands>()
                           .AddCommandClass<PackageCommands>());
            //Add the services.
            IServiceManager manager = IOCContainer.GetService<IServiceManager>();
            manager.AddServiceUsingBuilder(() =>
            {
                ServerStartup._remote = new RemoteImpl();
                MainRemotePlusServiceBuilder builder = new MainRemotePlusServiceBuilder(ServerStartup._remote);
                return builder.RouteHostClosedEvent(Host_Closed)
                        .RouteHostClosingEvent(Host_Closing)
                        .RouteHostFaultedEvent(Host_Faulted)
                        .RouteHostOpenEvent(Host_Opened)
                        .RouteHostOpeningEvent(Host_Opening)
                        .RouteUnknownMessageReceivedEvent(Host_UnknownMessageReceived)
                        .SetBinding(_ConnectionFactory.BuildBinding())
                        .SetPortNumber(ServerManager.DefaultSettings.PortNumber);
            });
            manager.AddServiceUsingBuilder(() =>
            {
                FileTransferServiceBuilder builder = new FileTransferServiceBuilder(typeof(FileTransferServiceImpl));
                return builder.RouteHostClosedEvent(Host_Closed)
                        .RouteHostClosingEvent(Host_Closing)
                        .RouteHostFaultedEvent(Host_Faulted)
                        .RouteHostOpenEvent(Host_Opened)
                        .RouteHostOpeningEvent(Host_Opening)
                        .RouteUnknownMessageReceivedEvent(Host_UnknownMessageReceived)
                        .SetBinding(_ConnectionFactory.BuildBinding())
                        .SetPortNumber(ServerManager.DefaultSettings.PortNumber);
            });
        }

        void IServerCoreStartup.InitializeServer(IServerBuilder builder)
        {
            builder.InitializeKnownTypes()
                .LoadServerConfig()
                .InitializeDefaultGlobals()
                .InitializeScriptingEngine((options) => { })
                .OpenMexForRemotePlus()
                .OpenMexForFileTransfer()
                .LoadGlobalResources()
                .InitializeVariables()
                .LoadExtensionLibraries();
        }
        public void PostInitializeServer(IServerBuilder builder)
        {
            builder.BuildServiceHost<ServerRemoteInterface>()
                .BuildServiceHost<FileTransferServciceInterface>();
        }
        #region Server Events
        private void Host_UnknownMessageReceived(object sender, System.ServiceModel.UnknownMessageReceivedEventArgs e)
        {
            GlobalServices.Logger.Log($"The server encountered an unknown message sent by the client. Message: {e.Message.ToString()}", LogLevel.Error);
        }

        private void Host_Opening(object sender, EventArgs e)
        {
            GlobalServices.Logger.Log("Opening server.", LogLevel.Info);
            ServerStartup._remote.SetRemoteInterface(ServerManager.ServerRemoteService);
        }

        private void Host_Opened(object sender, EventArgs e)
        {
            if (ServerManager.DefaultSettings.DiscoverySettings.DiscoveryBehavior == RemotePlusLibrary.Configuration.ServerSettings.ProxyConnectionMode.Connect)
            {
                GlobalServices.Logger.Log($"Host ready. Server is now part of the proxy cluster. Connect to proxy server to configure this server.", BetterLogger.LogLevel.Info);
            }
            else
            {
                GlobalServices.Logger.Log($"Host ready. Server is listening on port {ServerManager.DefaultSettings.PortNumber}. Connect to configure server.", BetterLogger.LogLevel.Info);
            }
        }

        private void Host_Faulted(object sender, EventArgs e)
        {
            GlobalServices.Logger.Log("The server state has been transferred to the faulted state.", LogLevel.Error);
        }

        private void Host_Closing(object sender, EventArgs e)
        {
            GlobalServices.Logger.Log("Closing the server.", BetterLogger.LogLevel.Info);
        }

        private void Host_Closed(object sender, EventArgs e)
        {
            GlobalServices.Logger.Log("The server is now closed.", BetterLogger.LogLevel.Info);
        }
        #endregion
    }
}