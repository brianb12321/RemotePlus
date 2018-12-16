using RemotePlusLibrary.Extension.ResourceSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusServer.Core
{
    public class RemotePlusResourceManager : IResourceManager
    {
        ResourceStore _store = ResourceStore.New();
        IResourceLoader _loader;
        public RemotePlusResourceManager(IResourceLoader loader)
        {
            _loader = loader;
        }
        public void AddResource<TResource>(TResource resource) where TResource : Resource
        {
            if(_store.ContainsKey(resource.ResourceIdentifier))
            {
                return;
            }
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

        public void Load()
        {
            _store = _loader.Load();
        }

        public void RemoveResource(string resourceID)
        {
            _store.Remove(resourceID);
        }

        public void Save()
        {
            _loader.Save(_store);
        }
    }
}