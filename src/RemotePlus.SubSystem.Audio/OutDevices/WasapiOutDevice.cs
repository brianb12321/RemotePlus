using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace RemotePlusLibrary.SubSystem.Audio.OutDevices
{
    public class WasapiOutDevice : AudioOutDevice
    {
        public static Func<string, WasapiOutDevice[]> Searcher = (name) =>
        {
            List<WasapiOutDevice> _devs = new List<WasapiOutDevice>();
            MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
            var defaultMM = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            var defaultConsole = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);
            var defaultComm = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Communications);
            _devs.Add(new WasapiOutDevice(name + "DefaultMM", defaultMM.DeviceFriendlyName, defaultMM.DeviceFriendlyName, defaultMM.ID, 2));
            _devs.Add(new WasapiOutDevice(name + "DefaultConsole", defaultConsole.DeviceFriendlyName, defaultConsole.DeviceFriendlyName, defaultConsole.ID, 2));
            _devs.Add(new WasapiOutDevice(name + "DefaultComm", defaultComm.DeviceFriendlyName, defaultComm.DeviceFriendlyName, defaultComm.ID, 2));
            int counter = 0;
            foreach(MMDevice dev in enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.All))
            {
                _devs.Add(new WasapiOutDevice(name + (counter + 1), dev.DeviceFriendlyName, dev.DeviceFriendlyName, dev.ID, 2));
                counter++;
            }
            return _devs.ToArray();
        };
        public override string ResourceType => "WASAPIRender";
        public AudioClientShareMode ShareMode => (AudioClientShareMode)DeviceProperties["ShareMode"].Value;
        public DeviceState State => (DeviceState)DeviceProperties["State"].Value;
        public int Latency => int.Parse(DeviceProperties["Latency"].Value.ToString());
        AudioClientShareMode _shareMode = AudioClientShareMode.Shared;
        MMDeviceEnumerator _enumerator = new MMDeviceEnumerator();
        public WasapiOutDevice(string id, string name, string description, string deviceID, int channels) : base(id, name, description, deviceID, channels)
        {
        }
        public override bool Init()
        {
            base.Init();
            DeviceProperties.Add("ShareMode", new Extension.ResourceSystem.ResourceTypes.DeviceProperty(false, false, "ShareMode"));
            DeviceProperties.Add("Latency", new Extension.ResourceSystem.ResourceTypes.DeviceProperty(true, true, "Latency"));
            DeviceProperties.Add("State", new Extension.ResourceSystem.ResourceTypes.DeviceProperty(false, false, "State"));
            DeviceProperties["ShareMode"].PropertyRead += () => _shareMode;
            DeviceProperties["Latency"].PropertyChanged += (sender, e) =>
            {
                if (int.TryParse(e.ToString(), out int result))
                {
                    return true;
                }
                else return false;
            };
            DeviceProperties["Latency"].SetValue(200);
            DeviceProperties["State"].PropertyRead += () => _enumerator.GetDevice(DeviceID)?.State;
            return true;
        }
        public override void InitProvider(IWaveProvider provider)
        {
            PlayerDevice = new WasapiOut(_enumerator.GetDevice(DeviceID), ShareMode, true, Latency);
            PlayerDevice.Init(provider);
        }

        public override void Write(byte[] data, int offset, int length)
        {
            InitProvider(new RawSourceWaveStream(data, offset, length, new WaveFormat()));
        }

        public override void BeginIO()
        {
            
        }
    }
}