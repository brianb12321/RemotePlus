using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Core.IOC;

namespace RemotePlusLibrary.Core
{
    public class WcfInstanceProviderAttribute : IInstanceProvider
    {
        private Type _instance = null;

        public WcfInstanceProviderAttribute(Type instance)
        {
            _instance = instance;
        }
        public object GetInstance(InstanceContext instanceContext)
        {
            return GetInstance(instanceContext, null);
        }

        public object GetInstance(InstanceContext instanceContext, Message message)
        {
            IClientContextExtensionProvider[] providers =
                IOCContainer.Provider.GetAllServices<IClientContextExtensionProvider>();
            foreach (var provider in providers)
            {
                provider.OnConnect(instanceContext, IOCContainer.Provider);
            }

            return Activator.CreateInstance(_instance);
        }

        public void ReleaseInstance(InstanceContext instanceContext, object instance)
        {
            IClientContextExtensionProvider[] providers =
                IOCContainer.Provider.GetAllServices<IClientContextExtensionProvider>();
            foreach (var provider in providers)
            {
                provider.OnDisconnect(instanceContext, IOCContainer.Provider);
            }
        }
    }
}