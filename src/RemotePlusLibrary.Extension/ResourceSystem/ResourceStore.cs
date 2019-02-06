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
        public ResourceDirectory GetResourceDirectoryByPath(string path)
        {
            return resources.GetResourceDirectoryByPath(path.Substring(1).Split('/'), 0, resources);
        }
        public void AddResourceDirectoryByPath(string path, string name)
        {
            if (path.Count() == 1 && path[0] == '/')
            {
                var newResourceDir = new ResourceDirectory(name);
                newResourceDir.Path = $"/{name}";
                resources.Directories.Add(name, newResourceDir);
            }
            else
            {
                resources.GetResourceDirectoryByPath(path.Substring(1).Split('/'), 0, resources).Directories.Add(name, new ResourceDirectory(name) { Path = path + $"/{name}"});
            }
        }
        public bool HasResourceByPath(string path)
        {
            return resources.HasResourceByPath(path.Substring(1).Split('/'), 0, resources);
        }
        public bool HasResourceByPathAndObject(string path, Resource res)
        {
            return resources.HasResourceByPath(((path.Substring(1) + $"/{res.ResourceIdentifier}").Split('/')), 0, resources);
        }
        public bool HasResourceDirectoryByPath(string path)
        {
            try
            {
                resources.GetResourceDirectoryByPath(path.Substring(1).Split('/'), 0, resources);
                return true;
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
        }
        public void AddResourceByPath(Resource resource, string path)
        {
            resource.Path = path + $"/{resource.ResourceIdentifier}";
            resources.GetResourceDirectoryByPath(path.Substring(1).Split('/'), 0, resources).AddResource(resource);
        }
        public void DeleteResourceByPath(string path)
        {
            //BAD CODE, but it works.
            string[] splittedPath = path.Substring(1).Split('/');
            resources.GetResourceDirectoryByPath(splittedPath.Take(splittedPath.Length - 1).ToArray(), 0, resources).Remove(splittedPath[splittedPath.Length - 1]);
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
            store.AddResourceDirectoryByPath("/", "custom");
            store.AddResourceDirectoryByPath("/", "dev");
            store.AddResourceDirectoryByPath("/dev", "utils");
            store.AddResourceDirectoryByPath("/dev", "io");
            store.AddResourceDirectoryByPath("/", "serverProperties");
            return store;
        }
        public static ResourceStore EmptyNew()
        {
            return new ResourceStore();
        }
    }
}
