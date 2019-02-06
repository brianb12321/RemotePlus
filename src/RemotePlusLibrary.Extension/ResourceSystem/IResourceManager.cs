using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.ResourceSystem
{
    public interface IResourceManager
    {
        TResource GetResource<TResource>(ResourceQuery query) where TResource : Resource;
        TResource[] GetAllResourcesByType<TResource>(string path) where TResource : Resource;
        void AddResource<TResource>(string path, TResource resource) where TResource : Resource;
        void AddResourceDirectory(string path, string name);
        IEnumerable<Resource> GetAllResources();
        void MoveResource(string originalPath, string newPath);
        void RemoveResource(string resourcePath);
        void Load();
        void Save();
    }
}