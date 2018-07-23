using RemotePlusLibrary.Core.IOC;
using Ninject;

namespace ProxyServer
{
    internal class ServiceCollection : IServiceCollection
    {
        public IServiceCollection AddSingleton<TService>(TService service)
        {
            IOCContainer.Provider.Bind<TService>().ToConstant(service);
            return this;
        }
        public IServiceCollection AddSingleton<TService>()
        {
            IOCContainer.Provider.Bind<TService>().ToSelf().InSingletonScope();
            return this;
        }

        public IServiceCollection AddSingleton<TService, TServiceImpl>()
        {
            IOCContainer.Provider.Bind<TService>().To(typeof(TServiceImpl)).InSingletonScope();
            return this;
        }

        public IServiceCollection AddSingletonNamed<TService, TServiceImpl>(string name)
        {
            IOCContainer.Provider.Bind<TService>().To(typeof(TServiceImpl)).Named(name);
            return this;
        }

        public IServiceCollection AddTransient<TService, TImplementation>()
        {
            IOCContainer.Provider.Bind<TService>().To(typeof(TImplementation)).InTransientScope();
            return this;
        }
        public IServiceCollection AddTransient<TService>()
        {
            IOCContainer.Provider.Bind<TService>().ToSelf().InTransientScope();
            return this;
        }
    }
}