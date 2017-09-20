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
    public class GlobalExceptionAttribute : Attribute, IServiceBehavior
    {
        private Type errorType = null;
        public GlobalExceptionAttribute(Type et)
        {
            errorType = et;
        }
        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
            
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            IErrorHandler handler = (IErrorHandler)Activator.CreateInstance(errorType);
            foreach(ChannelDispatcherBase b in serviceHostBase.ChannelDispatchers)
            {
                ChannelDispatcher cd = b as ChannelDispatcher;
                if(cd != null)
                {
                    cd.ErrorHandlers.Add(handler);
                }
            }
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            
        }
    }
}
