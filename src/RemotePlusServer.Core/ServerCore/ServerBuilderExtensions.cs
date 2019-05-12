using BetterLogger;
using RemotePlusLibrary.Configuration.ServerSettings;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.Scripting;
using RemotePlusLibrary.Security.AccountSystem;
using RemotePlusServer.Core.ExtensionSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel.Description;
using System.Speech.Synthesis;
using RemotePlusLibrary.Extension.ResourceSystem;
using RemotePlusLibrary.Extension.ResourceSystem.ResourceTypes;
using RemotePlusLibrary.Extension.ResourceSystem.ResourceTypes.Devices;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using RemotePlusLibrary.Extension;
using Ninject;
using Ninject.Planning.Bindings;
using RemotePlusLibrary.Core.NodeStartup;
using RemotePlusLibrary.ServiceArchitecture;
using RemotePlusLibrary.SubSystem.Command.CommandClasses;
using RemotePlusLibrary.SubSystem.Command;
using RemotePlusServer.Core.Commands;
using Binding = System.ServiceModel.Channels.Binding;

namespace RemotePlusServer.Core.ServerCore
{
    /// <summary>
    /// Provides helper methods for server core extensions to use to setup a custom server.
    /// </summary>
    public static class ServerBuilderExtensions
    {

        public static INodeBuilder<IServerTaskBuilder> ResolveLib(this INodeBuilder<IServerTaskBuilder> builder)
        {
            return builder.AddTask(() =>
            {
                AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            });
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs e)
        {
            string name = e.RequestingAssembly.GetName().Name;
            if (Directory.Exists("Libs") && Directory.Exists($"Libs\\{name}"))
            {
                var assembly = Assembly.LoadFrom($"Libs\\{name}\\{e.Name}");
                return assembly;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Sets the path to load the server settings.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="path">The path to set the search path to.</param>
        /// <returns></returns>
        public static INodeBuilder<IServerTaskBuilder> SetupServerConfigPath(this INodeBuilder<IServerTaskBuilder> builder, string path)
        {
            return builder.AddTask(() =>
            {
                ServerInitiliazationPipeline.ServerConfigPath = path;
            });
        }
        /// <summary>
        /// Initializes the default known types used in RemotePlus
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static INodeBuilder<IServerTaskBuilder> InitializeServerSideKnownTypes(this INodeBuilder<IServerTaskBuilder> builder)
        {
            return builder.AddTask(() =>
            {
                
            });
        }
        /// <summary>
        /// Initializes all the variables and functions pre-installed into RemotePlus scripting engine.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static INodeBuilder<IServerTaskBuilder> InitializeDefaultGlobals(this INodeBuilder<IServerTaskBuilder> builder)
        {
            return builder.AddTask(() =>
            {
                GlobalServices.Logger.Log("Initializing functions and variables.", LogLevel.Info, "Scripting Engine");
                InitializeGlobals();
            });
        }
        
        public static void InitializeGlobals()
        {
            try
            {
                IScriptExecutionContext context = IOCContainer.GetService<IScriptingEngine>().GetDefaultModule();
                context.AddVariable("executeServerCommand", new Func<IClientContext, string, CommandPipeline>((clientContext, command) => ServerManager.ServerRemoteService.RemoteInterface.RunServerCommand(clientContext, command, CommandExecutionMode.Script)));
                context.AddVariable("variableExists", new Func<string, bool>((name) => context.ContainsVariable(name)));
            }
            catch (ArgumentException)
            {

            }
        }

        /// <summary>
        /// Sets up the variable system for RemotePlus server.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static INodeBuilder<IServerTaskBuilder> InitializeVariables(this INodeBuilder<IServerTaskBuilder> builder)
        {
            return builder.AddTask(() =>
            {
                IResourceManager resourceManager = IOCContainer.GetService<IResourceManager>();
                resourceManager.AddResource("/serverProperties", new StringResource("Name", "RemotePlusServer"));
                IDeviceSearcher searcher = new DefaultDeviceSearcher();
                resourceManager.AddResource("/dev/utils", new TTSDevice());
                resourceManager.AddResource("/dev/utils", new NullDevice("null"));
                searcher.Get<KeyboardDevice>("keyboard").ToList().ForEach(d => resourceManager.AddResource("/dev/io", d));
                searcher.Get<MouseDevice>("mouse").ToList().ForEach(d => resourceManager.AddResource("/dev/io", d));
                IOCContainer.GetService<IScriptingEngine>().GetDefaultModule().AddVariable<Func<string, ResourceQuery>>("resq", (name) => new ResourceQuery(name, Guid.Empty));
            });
        }

        public static INodeBuilder<IServerTaskBuilder> OpenMexForRemotePlus(this INodeBuilder<IServerTaskBuilder> builder)
        {
            return builder.AddTask(() =>
            {
                if (ServerManager.DefaultSettings.EnableMetadataExchange)
                {
                    GlobalServices.Logger.Log("NOTE: Metadata exchange is enabled on the server.", LogLevel.Info, "Server Host");
                    Binding mexBinding = MetadataExchangeBindings.CreateMexTcpBinding();
                    ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
                    smb.HttpGetEnabled = true;
                    smb.HttpGetUrl = new Uri("http://0.0.0.0:9001/Mex");
                    ServerManager.ServerRemoteService.Host.Description.Behaviors.Add(smb);
                    ServerManager.ServerRemoteService.Host.AddServiceEndpoint(typeof(IMetadataExchange), mexBinding, "http://0.0.0.0:9001/Mex");
                }
            });
        }
        public static INodeBuilder<IServerTaskBuilder> OpenMexForFileTransfer(this INodeBuilder<IServerTaskBuilder> builder)
        {
            return builder.AddTask(() =>
            {
                if (ServerManager.DefaultSettings.EnableMetadataExchange)
                {
                    Binding mexBinding = MetadataExchangeBindings.CreateMexTcpBinding();
                    ServiceMetadataBehavior smb2 = new ServiceMetadataBehavior();
                    smb2.HttpGetEnabled = true;
                    smb2.HttpGetUrl = new Uri("http://0.0.0.0:9001/Mex2");
                    ServerManager.FileTransferService.Host.Description.Behaviors.Add(smb2);
                    ServerManager.FileTransferService.Host.AddServiceEndpoint(typeof(IMetadataExchange), mexBinding, "http://0.0.0.0:9001/Mex2");
                }
            });
        }
    }
}