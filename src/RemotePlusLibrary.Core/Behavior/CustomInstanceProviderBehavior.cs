using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace RemotePlusLibrary.Core.Behavior
{
    public class CustomInstanceProviderBehavior : IServiceBehavior
    {
        private Type _instanceProvider = null;
        private Type _instance = null;
        public CustomInstanceProviderBehavior(Type ip, Type instance)
        {
            _instanceProvider = ip;
            _instance = instance;
        }
        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            
        }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints,
            BindingParameterCollection bindingParameters)
        {
            
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (ChannelDispatcher cd in serviceHostBase.ChannelDispatchers)
            {
                foreach (EndpointDispatcher ed in cd.Endpoints)
                {
                    if (!ed.IsSystemEndpoint)
                    {
                        ed.DispatchRuntime.InstanceProvider = (IInstanceProvider)Activator.CreateInstance(_instanceProvider, _instance);
                    }
                }
            }
        }
    }
}
