using RemotePlusLibrary.Extension.ResourceSystem;
using RemotePlusLibrary.Extension.ResourceSystem.ResourceTypes;
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
        public void AddResource<TResource>(string path, TResource resource) where TResource : Resource
        {
            if (resource is IODevice)
            {
                bool success = (resource as IODevice).Init();
                if (!success)
                {
                    throw new Exception($"Device {resource.ResourceIdentifier} failed to initialize");
                }
            }
            if (ProxyManager.ResourceStore.HasResourceByPath(resource.ResourceIdentifier) || ProxyManager.ResourceStore.HasResourceByPathAndObject(path, resource))
            {
                return;
            }
            ProxyManager.ResourceStore.AddResourceByPath(resource, path);
        }

        public IEnumerable<Resource> GetAllResources()
        {
            return ProxyManager.ResourceStore.GetAllResources();
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

        public void MoveResource(string originalPath, string newPath)
        {
            Resource r = ProxyManager.ResourceStore.GetResourceByPath(originalPath);
            ProxyManager.ResourceStore.AddResourceByPath(r, newPath);
            ProxyManager.ResourceStore.DeleteResourceByPath(originalPath);
        }

        public void RemoveResource(string resourceID)
        {
            ProxyManager.ResourceStore.DeleteResourceByPath(resourceID);
        }

        public void Save()
        {
            _loader.Save(ProxyManager.ResourceStore);
        }
    }
}