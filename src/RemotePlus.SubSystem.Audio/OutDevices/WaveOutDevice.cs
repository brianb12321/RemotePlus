using NAudio.Wave;
using RemotePlusLibrary.Extension.ResourceSystem.ResourceTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.SubSystem.Audio.OutDevices
{
    /// <summary>
    /// Represents a wav out device.
    /// </summary>
    public class WaveOutDevice : AudioOutDevice
    {
        public override string ResourceType => "WaveOut";
        public static Func<string, WaveOutDevice[]> Searcher => (name) =>
        {
            List<WaveOutDevice> _devs = new List<WaveOutDevice>();
            WaveOutCapabilities defaultCap = WaveOut.GetCapabilities(-1);
            WaveOutDevice defaultDevice = new WaveOutDevice(name + "Default", defaultCap.ProductName, defaultCap.ProductName, "-1", defaultCap.Channels);
            _devs.Add(defaultDevice);
            for(int i = 0; i < WaveOut.DeviceCount; i++)
            {
                WaveOutCapabilities cap = WaveOut.GetCapabilities(i);
                WaveOutDevice device = new WaveOutDevice(name + (i + 1), cap.ProductName, cap.ProductName, i.ToString(), cap.Channels);
                _devs.Add(device);
            }
            return _devs.ToArray();
        };
        public WaveOutDevice(string id,
            string name,
            string description,
            string deviceID,
            int channels) : base(id, name, description, deviceID, channels)
        {
        }

        //This could be bad if you try to play a string.
        public override void Write(byte[] data, int offset, int length)
        {
            InitProvider(new RawSourceWaveStream(data, offset, length, new WaveFormat()));
            Play();
        }
        public override void InitProvider(IWaveProvider provider)
        {
            PlayerDevice = new WaveOutEvent();
            ((WaveOutEvent)PlayerDevice).DeviceNumber = int.Parse(DeviceID);
            PlayerDevice.Init(provider);
        }

        public override void BeginIO()
        {
        }
    }
}