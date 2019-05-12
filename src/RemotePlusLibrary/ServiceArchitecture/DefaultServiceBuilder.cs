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
    /// Provides the standord implementation of the Service Builder.
    /// </summary>
    public abstract class DefaultServiceBuilder<TInterface> : IWCFServiceBuilder<TInterface> where TInterface : class, new()
    {
        protected Binding _binding;
        protected EventHandler _hostOpened;
        protected EventHandler _hostOpening;
        protected EventHandler _hostClosed;
        protected EventHandler _hostClosing;
        protected EventHandler _hostFaulted;
        protected EventHandler<UnknownMessageReceivedEventArgs> _hostUnknown;
        protected int _portNumber = 9000;
        protected List<IServiceBehavior> _behaviors = new List<IServiceBehavior>();
        protected Dictionary<Type, List<IEndpointBehavior>> _endpointBehaviors = new Dictionary<Type, List<IEndpointBehavior>>();

        protected Dictionary<Type, List<IContractBehavior>> _contractBehaviors =
            new Dictionary<Type, List<IContractBehavior>>();
        public abstract IRemotePlusService<TInterface> BuildService();

        public virtual Binding GetBinding()
        {
            return _binding;
        }

        public virtual IWCFServiceBuilder<TInterface> RouteHostClosedEvent(EventHandler hostClosed)
        {
            _hostClosed += hostClosed;
            return this;
        }

        public virtual IWCFServiceBuilder<TInterface> RouteHostClosingEvent(EventHandler hostClosing)
        {
            _hostClosing += hostClosing;
            return this;
        }

        public virtual IWCFServiceBuilder<TInterface> RouteHostFaultedEvent(EventHandler hostFaulted)
        {
            _hostFaulted += hostFaulted;
            return this;
        }

        public virtual IWCFServiceBuilder<TInterface> RouteHostOpenEvent(EventHandler hostOpen)
        {
            _hostOpened += hostOpen;
            return this;
        }

        public virtual IWCFServiceBuilder<TInterface> RouteHostOpeningEvent(EventHandler hostOpening)
        {
            _hostOpening += hostOpening;
            return this;
        }

        public virtual IWCFServiceBuilder<TInterface> RouteUnknownMessageReceivedEvent(EventHandler<UnknownMessageReceivedEventArgs> eventHandler)
        {
            _hostUnknown += eventHandler;
            return this;
        }

        public virtual IWCFServiceBuilder<TInterface> SetBinding(Binding binding)
        {
            _binding = binding;
            return this;
        }

        public IWCFServiceBuilder<TInterface> AddServiceBehavior(IServiceBehavior behavior)
        {
            _behaviors.Add(behavior);
            return this;
        }

        public IWCFServiceBuilder<TInterface> AddEndpointBehavior<TEndpoint>(IEndpointBehavior behavior)
        {
            if (!_endpointBehaviors.ContainsKey(typeof(TEndpoint)))
            {
                _endpointBehaviors.Add(typeof(TEndpoint), new List<IEndpointBehavior>());
                _endpointBehaviors[typeof(TEndpoint)].Add(behavior);
            }
            else
            {
                _endpointBehaviors[typeof(TEndpoint)].Add(behavior);
            }

            return this;
        }

        public IWCFServiceBuilder<TInterface> AddContractBehavior<TContract>(IContractBehavior behavior)
        {
            if (!_contractBehaviors.ContainsKey(typeof(TContract)))
            {
                _contractBehaviors.Add(typeof(TContract), new List<IContractBehavior>());
                _contractBehaviors[typeof(TContract)].Add(behavior);
            }
            else
            {
                _contractBehaviors[typeof(TContract)].Add(behavior);
            }

            return this;
        }

        public virtual IWCFServiceBuilder<TInterface> SetPortNumber(int port)
        {
            _portNumber = port;
            return this;
        }

        public abstract IWCFServiceBuilder<TInterface> UseSingleton(object singleTon);
    }
}
