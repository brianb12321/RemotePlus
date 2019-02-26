using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.SubSystem.Audio.InDevices
{
    public class WaveInDevice : AudioInDevice
    {
        public static Func<string, WaveInDevice[]> Searcher = (name) =>
        {
            List<WaveInDevice> _devs = new List<WaveInDevice>();
            for (int i = 0; i < WaveIn.DeviceCount; i++)
            {
                WaveInCapabilities cap = WaveIn.GetCapabilities(i);
                _devs.Add(new WaveInDevice(name + (i + 1), cap.ProductName, cap.ProductName, i.ToString()));
            }
            return _devs.ToArray();
        };
        public override string ResourceType => "WaveIn";
        public WaveInDevice(string id, string name, string description, string deviceID) : base(id, name, description, deviceID)
        {
        }
        public override bool Init()
        {
            base.Init();
            WaveInDevice = new WaveInEvent();
            ((WaveInEvent)WaveInDevice).DeviceNumber = int.Parse(DeviceID);
            return true;
        }
    }
}