using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;

namespace RemotePlusLibrary.ServiceArchitecture
{
    public class DefaultServiceManager : IServiceManager
    {
        private IKernel serviceKernel = new StandardKernel();

        public void AddService<TService, TImpl>()
            where TService : IRemotePlusService<TImpl>
            where TImpl : new()
        {
            serviceKernel.Bind<IRemotePlusService<TImpl>>().To(typeof(TService)).InSingletonScope();
        }

        public IRemotePlusService<TImpl> GetService<TImpl>() where TImpl : new()
        {
            return serviceKernel.Get<IRemotePlusService<TImpl>>();
        }

        void IServiceManager.AddServiceUsingBuilder<TServiceImpl>(Func<IWCFServiceBuilder<TServiceImpl>> builder)
        {
            var b = builder?.Invoke();
            var service = b.BuildService();
            serviceKernel.Bind<IRemotePlusService<TServiceImpl>>().ToConstant(service);
        }
    }
}