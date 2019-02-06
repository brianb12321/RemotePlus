using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Linq;

namespace RemotePlusLibrary.Extension.ResourceSystem
{
    [DataContract]
    [Serializable]
    public class ResourceDirectory : ISerializable
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Path { get; set; }
        [DataMember]
        public Dictionary<string, Resource> Resources { get; set; } = new Dictionary<string, Resource>();
        [DataMember]
        public Dictionary<string, ResourceDirectory> Directories { get; set; } = new Dictionary<string, ResourceDirectory>();
        [DataMember]
        public bool SaveToFile { get; set; } = true;
        public ResourceDirectory(string name)
        {
            Name = name;
        }
        public ResourceDirectory(SerializationInfo info, StreamingContext context)
        {
            Directories = (Dictionary<string, ResourceDirectory>)info.GetValue("_storedDirectories", typeof(Dictionary<string, ResourceDirectory>));
            Resources = (Dictionary<string, Resource>)info.GetValue("_storedResources", typeof(Dictionary<string, Resource>));
            Name = (string)info.GetValue("Name", typeof(string));
            Path = (string)info.GetValue("Path", typeof(string));
        }
        
        public void AddDirectory(ResourceDirectory directory)
        {
            Directories.Add(directory.Name, directory);
        }
        public void AddResource(Resource resource)
        {
            Resources.Add(resource.ResourceIdentifier, resource);
        }
        public void Remove(string name)
        {
            Resources.Remove(name);
        }
        public Resource GetResource(string name)
        {
            return Resources[name];
        }
        public void GetAllResources(ResourceDirectory dir, List<Resource> buffer)
        {
            buffer.AddRange(dir.Resources.Values);
            foreach(ResourceDirectory dic in dir.Directories.Values)
            {
                GetAllResources(dic, buffer);
            }
        }
        public Resource GetResourceByName(ResourceDirectory dir, string resourceName)
        {
            if(dir.Resources.ContainsKey(resourceName))
            {
                return dir.Resources[resourceName];
            }
            else
            {
                return null;
            }
            
        }
        public Resource GetResourceByPath(string[] path, int position, ResourceDirectory dir)
        {
            //         0   1   2
            //path: $/dev/mnt/tcp
            if (path.Length == 1)
            {
                return dir.Resources[path[0]];
            }
            else
            {
                if(dir.Resources.ContainsKey(path[position]))
                {
                    return dir.Resources[path[position]];
                }
                else if(dir.Directories.ContainsKey(path[position]))
                {
                    ResourceDirectory foundIt = dir.Directories.Values.FirstOrDefault(d => d.Name == path[position]);
                    if(foundIt != null)
                    {
                        var foundResource = GetResourceByPath(path, ++position, foundIt);
                        if(foundResource != null)
                        {
                            return foundResource;
                        }
                        else
                        {
                            throw new KeyNotFoundException();
                        }
                    }
                    else
                    {
                        throw new KeyNotFoundException();
                    }
                }
                else
                {
                    throw new KeyNotFoundException();
                }
            }
        }
        public ResourceDirectory GetResourceDirectoryByPath(string[] path, int position, ResourceDirectory dir)
        {
            //         0   1   2
            //path: $/dev/mnt/tcp
            if (path.Length == 1)
            {
                return dir.Directories[path[0]];
            }
            else
            {
                if (dir.Directories.ContainsKey(path[position]))
                {
                    ResourceDirectory foundIt = dir.Directories.Values.FirstOrDefault(d => d.Name == path[position]);
                    if (path.Length - 1 == position)
                    {
                        if(foundIt != null)
                        {
                            return foundIt;
                        }
                        else
                        {
                            throw new KeyNotFoundException();
                        }
                    }
                    if (foundIt != null)
                    {
                        var foundResourceDir = GetResourceDirectoryByPath(path, ++position, foundIt);
                        if (foundResourceDir != null)
                        {
                            return foundResourceDir;
                        }
                        else
                        {
                            throw new KeyNotFoundException();
                        }
                    }
                    else
                    {
                        throw new KeyNotFoundException();
                    }
                }
                else
                {
                    throw new KeyNotFoundException();
                }
            }
        }
        public bool HasResourceByPath(string[] path, int position, ResourceDirectory dir)
        {
            try
            {
                GetResourceByPath(path, position, dir);
                return true;
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
        }
        public override string ToString()
        {
            return Path;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Dictionary<string, Resource> _temp = new Dictionary<string, Resource>();
            Dictionary<string, ResourceDirectory> _tempDirs = new Dictionary<string, ResourceDirectory>();
            foreach(Resource r in Resources.Values)
            {
                if(r.SaveToFile)
                {
                    _temp.Add(r.ResourceIdentifier, r);
                }
            }
            info.AddValue("_storedResources", _temp);
            foreach(ResourceDirectory dir in Directories.Values)
            {
                if(dir.SaveToFile)
                {
                    _tempDirs.Add(dir.Name, dir);
                }
            }
            info.AddValue("_storedDirectories", _tempDirs);
            info.AddValue("Path", Path);
            info.AddValue("Name", Name);
        }
    }
}