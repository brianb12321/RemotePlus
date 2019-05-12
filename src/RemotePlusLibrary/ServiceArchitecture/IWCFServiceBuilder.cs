using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.ServiceArchitecture
{
    /// <summary>
    /// Use to create and build a service
    /// </summary>
    public interface IWCFServiceBuilder<TInterface> where TInterface : class, new()
    {
        Binding GetBinding();
        IRemotePlusService<TInterface> BuildService();
        IWCFServiceBuilder<TInterface> RouteHostOpenEvent(EventHandler hostOpen);
        IWCFServiceBuilder<TInterface> RouteHostOpeningEvent(EventHandler hostOpening);
        IWCFServiceBuilder<TInterface> RouteHostClosedEvent(EventHandler hostClosed);
        IWCFServiceBuilder<TInterface> RouteHostClosingEvent(EventHandler hostClosing);
        IWCFServiceBuilder<TInterface> RouteHostFaultedEvent(EventHandler hostFaulted);
        IWCFServiceBuilder<TInterface> RouteUnknownMessageReceivedEvent(EventHandler<UnknownMessageReceivedEventArgs> eventHandler);
        IWCFServiceBuilder<TInterface> AddEndpointBehavior<TEndpoint>(IEndpointBehavior behavior);
        IWCFServiceBuilder<TInterface> AddContractBehavior<TContract>(IContractBehavior behavior);
        IWCFServiceBuilder<TInterface> UseSingleton(object singleTon);
        IWCFServiceBuilder<TInterface> SetPortNumber(int port);
        IWCFServiceBuilder<TInterface> SetBinding(Binding binding);
        IWCFServiceBuilder<TInterface> AddServiceBehavior(IServiceBehavior behavior);
    }
}