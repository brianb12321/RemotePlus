using System;
using BetterLogger;
using System.Windows.Forms;
using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.Core.EventSystem;

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
            return services.AddTransient<ICommandEnvironmnet, TCommandEnvironmentImpl>();
        }
        public static IServiceCollection UseEventBus<TEventBusImpl>(this IServiceCollection services) where TEventBusImpl : IEventBus
        {
            return services.AddSingleton<IEventBus, TEventBusImpl>();
        }
    }
}