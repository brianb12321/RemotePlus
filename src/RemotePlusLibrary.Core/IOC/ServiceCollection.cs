using System.Linq;
using Ninject;

namespace RemotePlusLibrary.Core.IOC
{
    public class ServiceCollection : IServiceCollection
    {
        private readonly IKernel _provider = new StandardKernel();
        public IServiceCollection AddSingleton<TService>(TService service)
        {
            _provider.Bind<TService>().ToConstant(service);
            return this;
        }

        public IServiceCollection AddSingleton<TService>()
        {
            _provider.Bind<TService>().ToSelf().InSingletonScope();
            return this;
        }
        public IServiceCollection AddSingleton<TService, TServiceImpl>()
        {
            _provider.Bind<TService>().To(typeof(TServiceImpl)).InSingletonScope();
            return this;
        }

        public IServiceCollection AddSingletonNamed<TService, TServiceImpl>(string name)
        {
            _provider.Bind<TService>().To(typeof(TServiceImpl)).Named(name);
            return this;
        }

        public IServiceCollection AddTransient<TService, TImplementation>()
        {
            _provider.Bind<TService>().To(typeof(TImplementation)).InTransientScope();
            return this;
        }

        public IServiceCollection AddTransient<TService>()
        {
            _provider.Bind<TService>().ToSelf().InTransientScope();
            return this;
        }

        public TService GetService<TService>()
        {
            return _provider.Get<TService>();
        }

        public TService GetService<TService>(string name)
        {
            return _provider.Get<TService>(name);
        }

        public TService[] GetAllServices<TService>()
        {
            return _provider.GetAll<TService>().ToArray();
        }
    }
}