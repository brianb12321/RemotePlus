using RemotePlusLibrary.Extension.ResourceSystem.ResourceTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.ResourceSystem
{
    public interface IDeviceSearcher
    {
        TDeviceType[] Get<TDeviceType>(string name) where TDeviceType : IODevice;
        void Add(string name, Func<string, IODevice[]> searcher);
    }
}