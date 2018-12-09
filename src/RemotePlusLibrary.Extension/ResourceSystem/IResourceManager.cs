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
        void AddResource<TResource>(TResource resource) where TResource : Resource;
        IEnumerable<Resource> GetAllResources();
    }
}