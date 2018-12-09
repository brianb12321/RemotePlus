using System;
using BetterLogger;
using System.Windows.Forms;
using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.Core.EventSystem;
using RemotePlusLibrary.ServiceArchitecture;
using RemotePlusLibrary.FileTransfer.Service.PackageSystem;
using RemotePlusLibrary.Extension.ExtensionLoader;
using System.Collections.Generic;
using System.ServiceModel.Dispatcher;
using RemotePlusLibrary.Extension.ResourceSystem;

namespace RemotePlusLibrary
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds a <see cref="ILogFactory"/> to the IOC container for use with RemotePlus.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options">Provides access to methods to add loggers.</param>
        /// <returns></returns>
        public static IServiceCollection UseLogger(this IServiceCollection services, Action<ILogFactory> options)
        {
            BaseLogFactory bf = new BaseLogFactory();
            options?.Invoke(bf);
            return services.AddSingleton<ILogFactory>(bf);
        }
        /// <summary>
        /// Adds a <see cref="IRemotePlusService{TRemoteInterface}"/> to the IOC container.
        /// </summary>
        /// <typeparam name="TServerInterface"></typeparam>
        /// <param name="services"></param>
        /// <param name="setup">Configures server specific settings like binding.</param>
        /// <returns></returns>
        public static IServiceCollection AddServer<TServerInterface>(this IServiceCollection services, Func<IRemotePlusService<TServerInterface>> setup) where TServerInterface : class, new()
        {
            var service = setup.Invoke();
            return services.AddSingleton(service);
        }
        public static IServiceCollection UseServerControlPage<TServerControl>(this IServiceCollection services) where TServerControl : Form
        {
            return services.AddTransient<Form, TServerControl>();
        }
        public static IServiceCollection UseScriptingEngine(this IServiceCollection services)
        {
            return services.AddSingleton<Scripting.ScriptBuilder>();
        }
        public static IServiceCollection UseConfigurationDataAccess<TDataAccessImpl>(this IServiceCollection services)
        {
            return services.AddSingletonNamed<Configuration.IConfigurationDataAccess, TDataAccessImpl>("DefaultConfigDataAccess");
        }
        public static IServiceCollection UseCommandline<TCommandEnvironmentImpl>(this IServiceCollection services, Action<CommandlineBuilder> builder) where TCommandEnvironmentImpl : ICommandEnvironmnet
        {
            var b = new CommandlineBuilder(services);
            builder?.Invoke(b);
            services.AddSingleton<ICommandClassStore, DefaultCommandStore>();
            return services.AddTransient<ICommandEnvironmnet, TCommandEnvironmentImpl>();       
        }
        public static IServiceCollection UseEventBus<TEventBusImpl>(this IServiceCollection services) where TEventBusImpl : IEventBus
        {
            return services.AddSingleton<IEventBus, TEventBusImpl>();
        }
        public static IServiceCollection UseServerManager<TServerManager>(this IServiceCollection services) where TServerManager : IServiceManager
        {
            return services.AddSingleton<IServiceManager, TServerManager>();
        }
        public static IServiceCollection UsePackageInventorySelector<TPackageInventorySelector>(this IServiceCollection services, Action<PackageSelectorBuilder> builder)
            where TPackageInventorySelector : IPackageInventorySelector, new()
        {
            var selectorInstance = new TPackageInventorySelector();
            var output = services.AddSingleton<IPackageInventorySelector>(selectorInstance);
            builder?.Invoke(new PackageSelectorBuilder(selectorInstance.PackageInventoryProvider));
            return output;
        }
        public static IServiceCollection UseExtensionContainer<TContainerImpl, TLibrary>(this IServiceCollection services, ExtensionLibraryCollectionBase<TLibrary> container)
            where TContainerImpl : ExtensionLibraryCollectionBase<TLibrary>
            where TLibrary : ExtensionLibraryBase
        {
            return services.AddSingleton(container);
        }
        public static IServiceCollection UseErrorHandler<TErrorHandler>(this IServiceCollection services) where TErrorHandler : IErrorHandler
        {
            return services.AddTransient<IErrorHandler, TErrorHandler>();
        }
        public static IServiceCollection UseResourceManager<TResourceManagerImpl>(this IServiceCollection services) where TResourceManagerImpl : IResourceManager
        {
            return services.AddSingleton<IResourceManager, TResourceManagerImpl>();
        }
    }
}