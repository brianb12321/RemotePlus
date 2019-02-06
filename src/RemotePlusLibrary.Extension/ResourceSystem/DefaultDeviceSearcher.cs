using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Extension.ResourceSystem.ResourceTypes;
using System.Management.Instrumentation;
using System.Management;
using RemotePlusLibrary.Extension.ResourceSystem.ResourceTypes.Devices;

namespace RemotePlusLibrary.Extension.ResourceSystem
{
    public class DefaultDeviceSearcher : IDeviceSearcher
    {
        Dictionary<string, Func<string, IODevice[]>> _searchers = new Dictionary<string, Func<string, IODevice[]>>();
        public DefaultDeviceSearcher()
        {
            _searchers.Add("keyboard", (name) =>
            {
                List<KeyboardDevice> _dev = new List<KeyboardDevice>();
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Keyboard");
                ManagementObjectCollection _results = searcher.Get();
                int count = 0;
                foreach(ManagementObject obj in _results)
                {
                    KeyboardDevice _newDev = new KeyboardDevice(name + (count + 1), obj["Name"].ToString(),
                        obj["Description"].ToString(),
                        obj["DeviceID"].ToString());
                    _dev.Add(_newDev);
                }
                return _dev.ToArray();
            });
            _searchers.Add("mouse", (name) =>
            {
                List<MouseDevice> _dev = new List<MouseDevice>();
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PointingDevice");
                ManagementObjectCollection _results = searcher.Get();
                int count = 0;
                foreach (ManagementObject obj in _results)
                {
                    MouseDevice _newDev = new MouseDevice(name + (count + 1), obj["Name"].ToString(),
                        obj["Description"].ToString(),
                        obj["DeviceID"].ToString());
                    _dev.Add(_newDev);
                }
                return _dev.ToArray();
            });
        }

        public void Add(string name, Func<string, IODevice[]> searcher)
        {
            _searchers.Add(name, searcher);
        }

        public TDeviceType[] Get<TDeviceType>(string name) where TDeviceType : IODevice
        {
            return (TDeviceType[])_searchers[name](name);
        }
    }
}