using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using RemotePlusLibrary.Core.IOC;

namespace RemotePlusLibrary.Core.Behavior
{
    public class GlobalExceptionBehavior : IServiceBehavior
    {
        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
            
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            IErrorHandler handler = IOCContainer.GetService<IErrorHandler>();
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
