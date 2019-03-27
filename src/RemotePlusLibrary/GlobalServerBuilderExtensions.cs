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

namespace RemotePlusLibrary
{
    public static class GlobalServerBuilderExtensions
    {
        public static IServerBuilder BuildServiceHost<TRemoteInterface>(this IServerBuilder builder)
            where TRemoteInterface : new()
        {
            return builder.AddTask(() =>
            {
                IServiceManager manager = IOCContainer.GetService<IServiceManager>();
                manager.BuildHost<TRemoteInterface>();
            });
        }
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
        public static IServerBuilder InitializeKnownTypes(this IServerBuilder builder)
        {
            return builder.AddTask(() => InitializeKnownTypes());
        }
        public static IServerBuilder InitializeKnownTypesByNamespace(this IServerBuilder builder, string name)
        {
            return builder.AddTask(() => InitializeKnownTypesByNamespace(name));
        }
        public static IServerBuilder LoadGlobalResources(this IServerBuilder builder)
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