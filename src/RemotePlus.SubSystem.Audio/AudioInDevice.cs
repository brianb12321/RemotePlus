using NAudio.Wave;
using RemotePlusLibrary.Extension.ResourceSystem.ResourceTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.SubSystem.Audio
{
    public class AudioInDevice : IODevice
    {
        protected NAudio.Wave.IWaveIn WaveInDevice { get; set; }
        public event EventHandler<StoppedEventArgs> RecordingStopped
        {
            add => WaveInDevice.RecordingStopped += value;
            remove => WaveInDevice.RecordingStopped -= value;
        }
        public event EventHandler<WaveInEventArgs> DataAvailable
        {
            add => WaveInDevice.DataAvailable += value;
            remove => WaveInDevice.DataAvailable -= value;
        }
        public AudioInDevice(string id, string name, string description, string deviceID) : base(id, name, description, deviceID)
        {
        }
        public void StartRecording()
        {
            WaveInDevice.StartRecording();
        }
        public void StopRecording()
        {
            WaveInDevice.StopRecording();
        }
        public void SetWaveFormat(WaveFormat format)
        {
            WaveInDevice.WaveFormat = format;
        }
        public override void BeginIO()
        {
            
        }
        public override bool Init()
        {
            return base.Init();
        }
        public override void Close()
        {
            if (WaveInDevice != null)
            {
                WaveInDevice.Dispose();
            }
        }
        public override void Shutdown()
        {
            base.Shutdown();
            if (WaveInDevice != null)
            {
                WaveInDevice.Dispose();
            }
        }

        public override Stream OpenReadStream()
        {
            throw new Exception("Cannot read from output device.");
        }

        public override Stream OpenWriteStream()
        {
            throw new NotImplementedException("Cannot write to output device.");
        }

        public override int Read(byte[] buffer, int offset, int length)
        {
            return 0;
        }

        public override void Write(byte[] data, int offset, int length)
        {
            
        }
    }
}