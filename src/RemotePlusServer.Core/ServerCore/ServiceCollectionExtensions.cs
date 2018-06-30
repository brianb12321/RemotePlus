using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary;
using BetterLogger;

namespace RemotePlusServer.Core.ServerCore
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection UseLogger(this IServiceCollection services, Action<ILogFactory> options)
        {
            BaseLogFactory bf = new BaseLogFactory();
            options?.Invoke(bf);
            return services.AddSingleton<ILogFactory>(bf);
        }
        public static IServiceCollection UseServer<TServerInterface>(this IServiceCollection services, Func<IRemotePlusService<TServerInterface>> setup) where TServerInterface : class, new()
        {
            var service = setup.Invoke();
            return services.AddSingleton(service);
        }
    }
}