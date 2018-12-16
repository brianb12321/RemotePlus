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
        private IResourceLoader _loader;
        public ProxyResourceManager(IResourceLoader loader)
        {
            _loader = loader;
        }
        public void AddResource<TResource>(TResource resource) where TResource : Resource
        {
            if (ProxyManager.ResourceStore.ContainsKey(resource.ResourceIdentifier))
            {
                return;
            }
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

        public void Load()
        {
            ProxyManager.ResourceStore = _loader.Load();
        }

        public void RemoveResource(string resourceID)
        {
            ProxyManager.ResourceStore.Remove(resourceID);
        }

        public void Save()
        {
            _loader.Save(ProxyManager.ResourceStore);
        }
    }
}