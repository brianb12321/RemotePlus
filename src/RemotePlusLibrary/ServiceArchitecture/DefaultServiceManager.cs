﻿using System;
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
            IOCContainer.Provider.Bind<IRemotePlusService<TImpl>>().To(typeof(TService)).InSingletonScope();
            ICommunicationObject communObject = IOCContainer.Provider.Get<IRemotePlusService<TImpl>>().Host;
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
            return IOCContainer.Provider.Get<IRemotePlusService<TImpl>>();
        }

        public CommunicationState GetState<TInterface>() where TInterface : new()
        {
            return IOCContainer.Provider.Get<IRemotePlusService<TInterface>>().Host.State;
        }
        
        void ConstructServices()
        {

        }
        void IServiceManager.AddServiceUsingBuilder<TServiceImpl>(Func<IWCFServiceBuilder<TServiceImpl>> builder)
        {
            var b = builder?.Invoke();
            var service = b.BuildService();
            IOCContainer.Provider.Bind<IRemotePlusService<TServiceImpl>>().ToConstant(service);
            GlobalServices.Logger.Log($"Service {typeof(TServiceImpl).Name} added to IOC.", BetterLogger.LogLevel.Info);
        }

        public void BuildHost<TImpl>() where TImpl : new()
        {
            var service = IOCContainer.Provider.Get<IRemotePlusService<TImpl>>();
            service.BuildHost();
            communicationObjects.Add(service.Host);
        }
    }
}