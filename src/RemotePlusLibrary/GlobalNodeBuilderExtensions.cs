using BetterLogger;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.Extension.ResourceSystem;
using RemotePlusLibrary.RequestSystem.DefaultRequestBuilders;
using RemotePlusLibrary.RequestSystem.DefaultRequestBuilders.BaseBuilders;
using RemotePlusLibrary.Scripting;
using RemotePlusLibrary.Security.AccountSystem;
using RemotePlusLibrary.ServiceArchitecture;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Core.NodeStartup;
using RemotePlusLibrary.Extension;

namespace RemotePlusLibrary
{
    public static class GlobalNodeBuilderExtensions
    {
        public static void InitializeKnownTypes()
        {
            GlobalServices.Logger.Log("Adding default known types.", LogLevel.Info);
            DefaultKnownTypeManager.LoadDefaultTypes();
            GlobalServices.Logger.Log("Adding UserAccount to known type list.", LogLevel.Debug);
            DefaultKnownTypeManager.AddType(typeof(UserAccount));
            DefaultKnownTypeManager.AddType(typeof(ResourceQuery));
            InitializeKnownTypesByNamespace("RemotePlusLibrary.RequestSystem.DefaultRequestBuilders");
            InitializeKnownTypesByNamespace("RemotePlusLibrary.RequestSystem.DefaultRequestBuilders.BaseBuilders");
            InitializeKnownTypesByNamespace("RemotePlusLibrary.SubSystem.Command.CommandClasses.Parsing.CommandElements");
            InitializeKnownTypesByNamespace("RemotePlusLibrary.Extension.ResourceSystem.ResourceTypes");
            InitializeKnownTypesByNamespace("RemotePlusLibrary.RequestSystem.DefaultUpdateRequestBuilders");
            InitializeKnownTypesByNamespace("RemotePlusLibrary.Core.EventSystem.Events");
            InitializeKnownTypesByNamespace("RemotePlusLibrary.Discovery.Events");
        }
        private static void InitializeKnownTypesByNamespace(string name)
        {
            GlobalServices.Logger.Log($"Adding default known types by namespace. Namespace: {name}", LogLevel.Debug);
            AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass && t.Namespace == name)
                .ToList()
                .ForEach(t =>
                {
                    GlobalServices.Logger.Log($"Known Type: {t.Name}", LogLevel.Debug);
                    DefaultKnownTypeManager.AddType(t);
                });
        }
        public static INodeBuilder<TBuilder> LoadExtensionLibraries<TBuilder>(this INodeBuilder<TBuilder> builder)
            where TBuilder : INodeBuilder<TBuilder>
        {
            return builder.AddTask(() =>
            {
                IExtensionLibraryLoader loader = IOCContainer.GetService<IExtensionLibraryLoader>();
                loader.LoadFromFolder("extensions");
            });
        }
        public static INodeBuilder<TBuilder> AddExceptionHandler<TBuilder>(this INodeBuilder<TBuilder> builder,
            UnhandledExceptionEventHandler handler) where TBuilder : INodeBuilder<TBuilder>
        {
            return builder.AddTask(() => { AppDomain.CurrentDomain.UnhandledException += handler; });
        }
        public static INodeBuilder<TBuilder> LoadExtensionByType<TBuilder>(this INodeBuilder<TBuilder> builder, Type loadType) where TBuilder : INodeBuilder<TBuilder>
        {
            return builder.AddTask(() =>
            {
                IOCContainer.GetService<IExtensionLibraryLoader>().LoadFromAssembly(Assembly.GetAssembly(loadType));
            });
        }
        public static INodeBuilder<TBuilder> LoadDefaultExtensionSubsystems<TBuilder, TSubsystem, TModule>(this INodeBuilder<TBuilder> builder)
            where TBuilder : INodeBuilder<TBuilder>
            where TSubsystem : IExtensionSubsystem<TModule>
            where TModule : IExtensionModule
        {
            return builder.AddTask(() =>
            {
                IOCContainer.Provider.GetService<TSubsystem>().Init();
            });
        }
        public static INodeBuilder<TBuilder> BuildServiceHost<TBuilder, TRemoteInterface>(this INodeBuilder<TBuilder> builder)
            where TRemoteInterface : new()
            where TBuilder : INodeBuilder<TBuilder>
        {
            return builder.AddTask(() =>
            {
                IServiceManager manager = IOCContainer.GetService<IServiceManager>();
                manager.BuildHost<TRemoteInterface>();
            });
        }
        public static INodeBuilder<TBuilder> InitializeKnownTypes<TBuilder>(this INodeBuilder<TBuilder> builder) where TBuilder : INodeBuilder<TBuilder>
        {
            return builder.AddTask(InitializeKnownTypes);
        }
        public static INodeBuilder<TBuilder> InitializeKnownTypesByNamespace<TBuilder>(this INodeBuilder<TBuilder> builder, string name) where TBuilder : INodeBuilder<TBuilder>
        {
            return builder.AddTask(() => InitializeKnownTypesByNamespace(name));
        }
        public static INodeBuilder<TBuilder> LoadGlobalResources<TBuilder>(this INodeBuilder<TBuilder> builder) where TBuilder : INodeBuilder<TBuilder>
        {
            var resourceManager = IOCContainer.GetService<IResourceManager>();
            return builder.AddTask(() =>
            {
                GlobalServices.Logger.Log("Loading global resources.", LogLevel.Info);
                try
                {
                    resourceManager.Load();
                }
                catch (FileNotFoundException)
                {
                    GlobalServices.Logger.Log("Global resource file or location does not exist. Creating new resource file or location.", LogLevel.Info);
                    resourceManager.Save();
                }
            });
        }
    }
}