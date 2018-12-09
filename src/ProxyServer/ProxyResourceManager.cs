using RemotePlusLibrary.Extension.ResourceSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyServer
{
    public class ProxyResourceManager : IResourceManager
    {
        public void AddResource<TResource>(TResource resource) where TResource : Resource
        {
            ProxyManager.ResourceStore.Add(resource.ResourceIdentifier, resource);
        }

        public IEnumerable<Resource> GetAllResources()
        {
            foreach (Resource r in ProxyManager.ResourceStore.Values)
            {
                yield return r;
            }
        }

        public TResource GetResource<TResource>(ResourceQuery query) where TResource : Resource
        {
            if(query.Node == Guid.Empty)
            {
                return (TResource)ProxyManager.ResourceStore[query.ResourceIdentifier];
            }
            if(ProxyManager.ProxyService.RemoteInterface.ConnectedServers.FirstOrDefault(s => s.UniqueID == query.Node) != null)
            {
                return (TResource)ProxyManager.ProxyService.RemoteInterface.ConnectedServers.FirstOrDefault(s => s.UniqueID == query.Node).ClientCallback.GetResource(query.ResourceIdentifier);
            }
            else
            {
                //Should throw exception.
                return null;
            }
        }
    }
}