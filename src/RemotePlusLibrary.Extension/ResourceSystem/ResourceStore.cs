using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace RemotePlusLibrary.Extension.ResourceSystem
{
    [DataContract]
    [Serializable]
    public class ResourceStore
    {
        [DataMember]
        ResourceDirectory resources;
        public Resource this[string path]
        {
            get
            {
                return GetResourceByPath(path);
            }
        }
        private ResourceStore()
        {
            resources = new ResourceDirectory("/");
            resources.Path = "/";
        }
        public Resource GetResourceByPath(string path)
        {
            return resources.GetResourceByPath(path.Substring(1).Split('/'), 0, resources);
        }
        public void AddResourceDirectoryByPath(string path, string name)
        {
            if (path[0] == '/')
            {
                var newResourceDir = new ResourceDirectory(name);
                newResourceDir.Path = $"/{name}";
                resources.Directories.Add(name, newResourceDir);
            }
            else
            {
                resources.GetResourceDirectoryByPath(path.Substring(1).Split('/'), 0, resources).Directories.Add(name, new ResourceDirectory(name));
            }
        }
        public bool HasResourceByPath(string path)
        {
            return resources.HasResourceByPath(path.Substring(1).Split('/'), 0, resources);
        }
        public void AddResourceByPath(Resource resource, string path)
        {
            resource.Path = path + $"/{resource.ResourceIdentifier}";
            resources.GetResourceDirectoryByPath(path.Substring(1).Split('/'), 0, resources).AddResource(resource);
        }
        public void DeleteResourceByPath(string path)
        {
            string[] splittedPath = path.Substring(1).Split('/');
            resources.GetResourceDirectoryByPath(splittedPath, 0, resources).Remove(splittedPath[splittedPath.Length - 1]);
        }
        public IEnumerable<Resource> GetAllResources()
        {
            List<Resource> _resources = new List<Resource>();
            resources.GetAllResources(resources, _resources);
            return _resources;
        }
        public static ResourceStore New()
        {
            ResourceStore store = new ResourceStore();
            store.AddResourceDirectoryByPath("/", "temp");
            store.AddResourceDirectoryByPath("/", "stuff");
            store.AddResourceDirectoryByPath("/", "custom");
            return store;
        }
    }
}
