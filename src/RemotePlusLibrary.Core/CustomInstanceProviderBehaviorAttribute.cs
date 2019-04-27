using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Core
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomInstanceProviderBehaviorAttribute : Attribute, IServiceBehavior
    {
        private Type _instanceProvider = null;
        private Type _instance = null;
        public CustomInstanceProviderBehaviorAttribute(Type ip, Type instance)
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
