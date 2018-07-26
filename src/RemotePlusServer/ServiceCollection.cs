using RemotePlusLibrary;
using RemotePlusServer.Core.ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using RemotePlusLibrary.Core.IOC;

namespace RemotePlusServer
{
    public class ServiceCollection : IServiceCollection
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
