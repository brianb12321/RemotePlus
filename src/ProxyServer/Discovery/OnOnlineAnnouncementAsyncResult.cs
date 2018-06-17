using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Discovery;
using System.ServiceModel;
using System.IdentityModel;

namespace ProxyServer.Discovery
{
    sealed class OnOnlineAnnouncementAsyncResult : TypedAsyncResult<OnOnlineAnnouncementAsyncResult>
    {
        public OnOnlineAnnouncementAsyncResult(AsyncCallback callback, object state) : base(callback, state)
        {
            this.Complete(true);
        }
        public static new void End(IAsyncResult result)
        {
            TypedAsyncResult<OnOnlineAnnouncementAsyncResult>.End(result);
        }
    }
    sealed class OnOfflineAnnouncementAsyncResult : TypedAsyncResult<OnOfflineAnnouncementAsyncResult>
    {
        public OnOfflineAnnouncementAsyncResult(AsyncCallback callback, object state) : base(callback, state)
        {
            this.Complete(true);
        }
        public static new void End(IAsyncResult result)
        {
            TypedAsyncResult<OnOfflineAnnouncementAsyncResult>.End(result);
        }
    }
    sealed class OnFindAsyncResult : TypedAsyncResult<OnFindAsyncResult>
    {
        public OnFindAsyncResult(AsyncCallback callback, object state) : base(callback, state)
        {
            this.Complete(true);
        }
        public static new void End(IAsyncResult result)
        {
            TypedAsyncResult<OnFindAsyncResult>.End(result);
        }
    }
    sealed class OnResolveAsyncResult : TypedAsyncResult<OnResolveAsyncResult>
    {
        EndpointDiscoveryMetadata matchingEndpoint;
        public OnResolveAsyncResult(EndpointDiscoveryMetadata me, AsyncCallback callback, object state) : base(callback, state)
        {
            this.matchingEndpoint = me;
            this.Complete(true);
        }
        public static new EndpointDiscoveryMetadata End(IAsyncResult result)
        {
            OnResolveAsyncResult thisPtr = TypedAsyncResult<OnResolveAsyncResult>.End(result);
            return thisPtr.matchingEndpoint;
        }
    }
}
