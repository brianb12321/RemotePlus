using System;
using System.Collections.Generic;
using System.ServiceModel;
using Ninject;

namespace RemotePlusLibrary.ServiceArchitecture
{
    public class DefaultServiceManager : IServiceManager
    {
        private IKernel serviceKernel = new StandardKernel();
        private List<ICommunicationObject> communicationObjects = new List<ICommunicationObject>();
        public void AbortAll()
        {
            foreach (ICommunicationObject commObject in communicationObjects)
            {
                commObject.Abort();
            }
        }

        public void AddService<TService, TImpl>()
            where TService : IRemotePlusService<TImpl>
            where TImpl : new()
        {
            serviceKernel.Bind<IRemotePlusService<TImpl>>().To(typeof(TService)).InSingletonScope();
            ICommunicationObject communObject = serviceKernel.Get<IRemotePlusService<TImpl>>().Host;
            communicationObjects.Add(communObject);
        }

        public void CloseAll()
        {
            foreach(ICommunicationObject commObject in communicationObjects)
            {
                commObject.Close();
            }
        }

        public IRemotePlusService<TImpl> GetService<TImpl>() where TImpl : new()
        {
            return serviceKernel.Get<IRemotePlusService<TImpl>>();
        }

        public CommunicationState GetState<TInterface>() where TInterface : new()
        {
            return serviceKernel.Get<IRemotePlusService<TInterface>>().Host.State;
        }
        
        void ConstructServices()
        {

        }
        void IServiceManager.AddServiceUsingBuilder<TServiceImpl>(Func<IWCFServiceBuilder<TServiceImpl>> builder)
        {
            var b = builder?.Invoke();
            var service = b.BuildService();
            serviceKernel.Bind<IRemotePlusService<TServiceImpl>>().ToConstant(service);
        }

        public void BuildHost<TImpl>() where TImpl : new()
        {
            var service = serviceKernel.Get<IRemotePlusService<TImpl>>();
            service.BuildHost();
            communicationObjects.Add(service.Host);
        }
    }
}