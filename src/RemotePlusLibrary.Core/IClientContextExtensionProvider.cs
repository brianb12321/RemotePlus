using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Core.IOC;

namespace RemotePlusLibrary.Core
{
    /// <summary>
    /// Gets called from an instance provider when a client connects or disconnects;
    /// </summary>
    public interface IClientContextExtensionProvider
    {
        void OnConnect(InstanceContext context, IServiceCollection services);
        void OnDisconnect(InstanceContext context, IServiceCollection services);
    }
}