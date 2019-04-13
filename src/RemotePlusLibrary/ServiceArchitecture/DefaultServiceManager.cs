using System;
using System.Collections.Generic;
using System.ServiceModel;
using Ninject;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Core.IOC;

namespace RemotePlusLibrary.ServiceArchitecture
{
    public class DefaultServiceManager : IServiceManager
    {
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
            IOCContainer.Provider.AddSingleton<IRemotePlusService<TImpl>, TService>();
            ICommunicationObject communObject = IOCContainer.Provider.GetService<IRemotePlusService<TImpl>>().Host;
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
            return IOCContainer.Provider.GetService<IRemotePlusService<TImpl>>();
        }

        public CommunicationState GetState<TInterface>() where TInterface : new()
        {
            return IOCContainer.Provider.GetService<IRemotePlusService<TInterface>>().Host.State;
        }
        
        void ConstructServices()
        {

        }
        void IServiceManager.AddServiceUsingBuilder<TServiceImpl>(Func<IWCFServiceBuilder<TServiceImpl>> builder)
        {
            var b = builder?.Invoke();
            var service = b.BuildService();
            IOCContainer.Provider.AddSingleton(service);
            GlobalServices.Logger.Log($"Service {typeof(TServiceImpl).Name} added to IOC.", BetterLogger.LogLevel.Info);
        }

        public void BuildHost<TImpl>() where TImpl : new()
        {
            var service = IOCContainer.Provider.GetService<IRemotePlusService<TImpl>>();
            service.BuildHost();
            communicationObjects.Add(service.Host);
        }
    }
}