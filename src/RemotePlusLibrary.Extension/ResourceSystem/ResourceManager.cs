using RemotePlusLibrary.Extension.ResourceSystem;
using RemotePlusLibrary.Extension.ResourceSystem.ResourceTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.ResourceSystem
{
    public class RemotePlusResourceManager : IResourceManager
    {
        ResourceStore _store = ResourceStore.New();
        IResourceLoader _loader;
        public RemotePlusResourceManager(IResourceLoader loader)
        {
            _loader = loader;
        }
        public void AddResource<TResource>(string path, TResource resource) where TResource : Resource
        {
            if(_store.HasResourceByPathAndObject(path, resource))
            {
                return;
            }
            if(resource is IODevice)
            {
                bool success = (resource as IODevice).Init();
                if(!success)
                {
                    throw new Exception($"Device {resource.ResourceIdentifier} failed to initialize");
                }
            }
            _store.AddResourceByPath(resource, path);
        }

        public void AddResourceDirectory(string path, string name)
        {
            if(_store.HasResourceDirectoryByPath(path + $"/{name}"))
            {
                return;
            }
            _store.AddResourceDirectoryByPath(path, name);
        }

        public IEnumerable<Resource> GetAllResources()
        {
            return _store.GetAllResources();
        }

        public TResource[] GetAllResourcesByType<TResource>(string path) where TResource : Resource
        {
            return _store.GetResourceDirectoryByPath(path).Resources.Values
                .Where(r => r is TResource)
                .Select(r => (TResource)r)
                .ToArray();
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

        public void MoveResource(string originalPath, string newPath)
        {
            Resource r =_store.GetResourceByPath(originalPath);
            _store.AddResourceByPath(r, newPath);
            _store.DeleteResourceByPath(originalPath);
        }

        public void RemoveResource(string resourcePath)
        {
            _store.DeleteResourceByPath(resourcePath);
        }

        public void Save()
        {
            _loader.Save(_store);
        }
    }
}