using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using BetterLogger;
using RemotePlusServer.Core;
using RemotePlusServer.Core.ServerCore;
using RemotePlusServer;
using RemotePlusLibrary;
using RemotePlusLibrary.ServiceArchitecture;
using BetterLogger.Loggers;
using RemotePlusLibrary.Configuration;
using RemotePlusLibrary.Configuration.StandordDataAccess;
using RemotePlusLibrary.Contracts;
using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.Security.AccountSystem;
using RSPM;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Core.Behavior;
using RemotePlusLibrary.Core.NodeStartup;
using RemotePlusLibrary.Discovery;
using RemotePlusServer.Core.ExtensionSystem;
using RemotePlusLibrary.Extension.ResourceSystem;
using RemotePlusLibrary.Scripting;
using RemotePlusLibrary.SubSystem.Audio;
using RemotePlusLibrary.Extension;
using RemotePlusLibrary.SubSystem.Command;
using RemotePlusLibrary.SubSystem.Command.CommandClasses.Parsing;
using RemotePlusLibrary.SubSystem.Command.CommandClasses;
using RemotePlusLibrary.SubSystem.Workflow;
using RemotePlusLibrary.SubSystem.Workflow.ExtensionSystem;
using RemotePlusLibrary.SubSystem.Workflow.Server;
using RemotePlusServer.Core.Commands;

namespace DefaultServerCore
{
    public sealed class Startup : IServerCoreStartup, IClientCoreStartup
    {
        public void AddServices(IServiceCollection services)
        {
            services.UseConfigurationDataAccess<ConfigurationHelper>()
                .AddSingletonNamed<IConfigurationDataAccess, BinarySerializationHelper>("BinaryDataAccess")
                .UseLogger(logFactory =>
            {
                logFactory.AddLogger(new ConsoleLogger()
                {
                    Settings = new ConsoleLoggerOptions()
                    {
                        VerboseColor = ConsoleColor.DarkMagenta
                    }
                });
            });
            services.UseAuthentication<AccountManager>();
            services.LoadServerSettings();
            services.UseServerManager<DefaultServiceManager>()
                .UseResourceManager<RemotePlusResourceManager, FileResourceLoader>()
                .UseErrorHandler<GlobalErrorHandler>()
                .UseExtensionSystem<DefaultExtensionLoader>()
                .UseServerControlPage<ServerControls>()
                .UseScriptingEngine<IronPythonScriptingEngine>()
                .UsePackageManager<DefaultPackageManager>()
                .UseCommandline<CommandEnvironment, ServerCommandSubsystem, IServerCommandModule>(builder =>
                    builder.UseLexer<CommandLexer>()
                           .UseParser<CommandParser>()
                           .UseExecutor<CommandExecutor>())
                .AddWorkflowFeature();
            //Add the services.
            IServiceManager manager = services.GetService<IServiceManager>();
            manager.AddServiceUsingBuilder(() =>
            {
                MainRemotePlusServiceBuilder builder = new MainRemotePlusServiceBuilder(typeof(RemoteImpl));
                return builder.RouteHostClosedEvent(Host_Closed)
                        .RouteHostClosingEvent(Host_Closing)
                        .RouteHostFaultedEvent(Host_Faulted)
                        .RouteHostOpenEvent(Host_Opened)
                        .RouteHostOpeningEvent(Host_Opening)
                        .RouteUnknownMessageReceivedEvent(Host_UnknownMessageReceived)
                        .SetBinding(_ConnectionFactory.BuildBinding())
                        .SetPortNumber(ServerManager.DefaultSettings.PortNumber)
                        .AddServiceBehavior(new GlobalExceptionBehavior())
                        .AddServiceBehavior(new CustomInstanceProviderBehavior(typeof(WcfInstanceProvider), typeof(RemoteImpl)))
                        .AddContractBehavior<IRemote>(new NetDataContractSerializerBehavior());
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
                        .SetPortNumber(ServerManager.DefaultSettings.PortNumber)
                        .AddServiceBehavior(new GlobalExceptionBehavior());
            });
        }

        public void InitializeClientServices(IServiceCollection services)
        {
            DuplexChannelFactory<IProxyServerRemote> channelFactory =
                services.GetService<DuplexChannelFactory<IProxyServerRemote>>();
            channelFactory.Endpoint.Contract.Operations.ToList().ForEach(o =>
            {
                DataContractSerializerOperationBehavior behavior =
                    o.Behaviors.Find<DataContractSerializerOperationBehavior>();
                o.Behaviors.Remove(behavior);
                o.Behaviors.Add(new NetDataContractOperationBehavior(o));
            });
        }

        public void InitializeNode(IClientBuilder builder)
        {
            
        }

        public void PostInitializeNode(IClientBuilder builder)
        {
            throw new NotImplementedException();
        }

        public void InitializeNode(IServerTaskBuilder builder)
        {
            //builder.InitializeKnownTypes()
            builder.InitializeDefaultGlobals()
                .OpenMexForRemotePlus()
                .OpenMexForFileTransfer()
                .LoadGlobalResources()
                .AddAudioDevices()
                .InitializeVariables()
                .ResolveLib()
                .LoadExtensionLibraries()
                .LoadExtensionByType(typeof(DefaultCommands))
                .LoadExtensionByType(typeof(PackageCommands))
                .LoadExtensionByType(typeof(AudioCommands))
                .LoadExtensionByType(typeof(RemotePlusActivityContext))
                .LoadExtensionByType(typeof(WorkflowCommands));
        }

        public void PostInitializeNode(IServerTaskBuilder builder)
        {
            builder.BuildServiceHost<IServerTaskBuilder, ServerRemoteInterface>()
                .BuildServiceHost<IServerTaskBuilder, FileTransferServciceInterface>()
                .LoadDefaultExtensionSubsystems<IServerTaskBuilder, ICommandSubsystem<IServerCommandModule>, IServerCommandModule>();
        }
        #region Server Events
        private void Host_UnknownMessageReceived(object sender, UnknownMessageReceivedEventArgs e)
        {
            GlobalServices.Logger.Log($"The server encountered an unknown message sent by the client. Message: {e.Message}", LogLevel.Error);
        }

        private void Host_Opening(object sender, EventArgs e)
        {
            GlobalServices.Logger.Log("Opening server.", LogLevel.Info);
        }

        private void Host_Opened(object sender, EventArgs e)
        {
            if (ServerManager.DefaultSettings.DiscoverySettings.DiscoveryBehavior == RemotePlusLibrary.Configuration.ServerSettings.ProxyConnectionMode.Connect)
            {
                GlobalServices.Logger.Log($"Host ready. Server is now part of the proxy cluster. Connect to proxy server to configure this server.", LogLevel.Info);
            }
            else
            {
                GlobalServices.Logger.Log($"Host ready. Server is listening on port {ServerManager.DefaultSettings.PortNumber}. Connect to configure server.", LogLevel.Info);
            }
        }

        private void Host_Faulted(object sender, EventArgs e)
        {
            GlobalServices.Logger.Log("The server state has been transferred to the faulted state.", LogLevel.Error);
        }

        private void Host_Closing(object sender, EventArgs e)
        {
            GlobalServices.Logger.Log("Closing the server.", LogLevel.Info);
        }

        private void Host_Closed(object sender, EventArgs e)
        {
            GlobalServices.Logger.Log("The server is now closed.", LogLevel.Info);
        }
        #endregion
    }
}