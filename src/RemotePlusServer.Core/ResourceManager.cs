using RemotePlusLibrary.Extension.ResourceSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusServer.Core
{
    public class ResourceManager : IResourceManager
    {
        ResourceStore _store = ResourceStore.New();
        public void AddResource<TResource>(TResource resource) where TResource : Resource
        {
            _store.Add(resource.ResourceIdentifier, resource);
        }

        public IEnumerable<Resource> GetAllResources()
        {
            foreach(Resource r in _store.Values)
            {
                yield return r;
            }
        }

        public TResource GetResource<TResource>(ResourceQuery query) where TResource : Resource
        {
            try
            {
                return (TResource)_store[query.ResourceIdentifier];
            }
            catch (KeyNotFoundException)
            {
                throw new Exception($"Resource {query.ResourceIdentifier} does not exist.");
            }
            catch (InvalidCastException)
            {
                throw new Exception("Resource type does not match.");
            }
        }

        public void RemoveResource(string resourceID)
        {
            _store.Remove(resourceID);
        }
    }
}