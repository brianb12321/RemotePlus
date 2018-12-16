using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.ResourceSystem
{
    public interface IResourceLoader
    {
        ResourceStore Load();
        void Save(ResourceStore store);
    }
}