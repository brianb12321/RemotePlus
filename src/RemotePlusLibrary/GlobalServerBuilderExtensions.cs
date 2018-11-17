using BetterLogger;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.RequestSystem.DefaultRequestBuilders;
using RemotePlusLibrary.RequestSystem.DefaultRequestBuilders.BaseBuilders;
using RemotePlusLibrary.Security.AccountSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary
{
    public static class GlobalServerBuilderExtensions
    {
        public static void InitializeKnownTypes()
        {
            GlobalServices.Logger.Log("Adding default known types.", LogLevel.Info);
            DefaultKnownTypeManager.LoadDefaultTypes();
            GlobalServices.Logger.Log("Adding UserAccount to known type list.", LogLevel.Debug);
            DefaultKnownTypeManager.AddType(typeof(UserAccount));
            InitializeKnownTypesByNamespace("RemotePlusLibrary.RequestSystem.DefaultRequestBuilders");
            InitializeKnownTypesByNamespace("RemotePlusLibrary.RequestSystem.DefaultRequestBuilders.BaseBuilders");
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
    }
}
