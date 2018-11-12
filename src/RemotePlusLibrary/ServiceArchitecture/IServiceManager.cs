using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.ServiceArchitecture
{
    public interface IServiceManager
    {
        /// <summary>
        /// Adds a WCF service to the service manager.
        /// </summary>
        /// <typeparam name="TService">The implementation of the service.</typeparam>
        /// <typeparam name="TImpl">The service interface used to identify the service.</typeparam>
        void AddService<TService, TImpl>() where TService : IRemotePlusService<TImpl> where TImpl : new();
        /// <summary>
        /// Adds a WCF service using a service builder.
        /// </summary>
        /// <typeparam name="TServiceImpl">The service interface used to identify the builder.</typeparam>
        /// <param name="builder">The function called to build the builder.</param>
        void AddServiceUsingBuilder<TServiceImpl>(Func<IWCFServiceBuilder<TServiceImpl>> builder) where TServiceImpl : class, new();
        /// <summary>
        /// Gets the specified WCF service from the manager.
        /// </summary>
        /// <typeparam name="TImpl">The service interface used to identify the service.</typeparam>
        /// <returns>The service.</returns>
        IRemotePlusService<TImpl> GetService<TImpl>() where TImpl : new();
        void BuildHost<TImpl>() where TImpl : new();
        /// <summary>
        /// Closes all the services managed by the manager.
        /// </summary>
        void CloseAll();
        /// <summary>
        /// Aborts all the services managed by the manager.
        /// </summary>
        void AbortAll();
        /// <summary>
        /// Gets the current state of the specified service.
        /// </summary>
        /// <typeparam name="TInterface">The interface used to identify the service.</typeparam>
        /// <returns></returns>
        System.ServiceModel.CommunicationState GetState<TInterface>() where TInterface : new();
    }
}