using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
using RemotePlusLibrary.Extension.ResourceSystem.ResourceTypes;

namespace RemotePlusLibrary.SubSystem.Audio.OutDevices
{
    public class DirectSoundOutDevice : AudioOutDevice
    {
        public static Func<string, DirectSoundOutDevice[]> Searcher = (name) =>
        {
            List<DirectSoundOutDevice> _devs = new List<DirectSoundOutDevice>();
            try
            {
                int counter = 0;
                foreach (DirectSoundDeviceInfo info in DirectSoundOut.Devices)
                {
                    DirectSoundOutDevice dev = new DirectSoundOutDevice(name + (counter + 1), info.ModuleName, info.Description, info.Guid.ToString(), 2);
                    _devs.Add(dev);
                }
            }
            catch (System.Runtime.InteropServices.COMException)
            {

            }
            return _devs.ToArray();
        };
        public override string ResourceType => "DirectSoundOut";
        public DirectSoundOutDevice(string id, string name, string description, string deviceID, int channels) : base(id, name, description, deviceID, channels)
        {
        }
        public override void InitProvider(IWaveProvider provider)
        {
            PlayerDevice = new DirectSoundOut(Guid.Parse(DeviceID));
            PlayerDevice.Init(provider);
        }

        public override void Write(byte[] data, int offset, int length)
        {
            var source = new RawSourceWaveStream(data, offset, length, new WaveFormat());
            InitProvider(source);
        }

        public override void BeginIO()
        {
            
        }
    }
}