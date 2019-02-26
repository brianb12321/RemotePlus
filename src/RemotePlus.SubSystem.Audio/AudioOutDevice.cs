using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.SubSystem.Audio
{
    public abstract class AudioOutDevice : AudioDevice
    {
        protected IWavePlayer PlayerDevice { get; set; }
        public PlaybackState PlaybackState => (PlaybackState)DeviceProperties["PlaybackState"].Value;
        public event EventHandler<StoppedEventArgs> PlaybackStopped
        {
            add => PlayerDevice.PlaybackStopped += value;
            remove => PlayerDevice.PlaybackStopped -= value;
        }
        public AudioOutDevice(string id, string name, string description, string deviceID, int channels) : base(id, name, description, deviceID, channels)
        {
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
            throw new NotImplementedException("Cannot read from output device.");
        }

        public override abstract void Write(byte[] data, int offset, int length);
        public abstract void InitProvider(IWaveProvider provider);
        public override void Close()
        {
            if (PlayerDevice != null)
            {
                PlayerDevice.Dispose();
            }
        }
        public override void Shutdown()
        {
            base.Shutdown();
            if (PlayerDevice != null)
            {
                PlayerDevice.Dispose();
            }
        }
        public virtual void Play()
        {
            PlayerDevice.Play();
        }
        public virtual void Stop()
        {
            PlayerDevice.Stop();
        }
        public override bool Init()
        {
            base.Init();
            DeviceProperties.Add("PlaybackState", new RemotePlusLibrary.Extension.ResourceSystem.ResourceTypes.DeviceProperty(false, false, "PlaybackState"));
            DeviceProperties.Add("WavePlayerVolume", new Extension.ResourceSystem.ResourceTypes.DeviceProperty(false, true, "WavePlayerVolume"));
            DeviceProperties["PlaybackState"].PropertyRead += () => PlayerDevice?.PlaybackState ?? PlaybackState.Stopped;
            DeviceProperties["WavePlayerVolume"].PropertyRead += () => PlayerDevice?.Volume;
            DeviceProperties["WavePlayerVolume"].PropertyChanged += (sender, value) =>
            {
                if (!float.TryParse(value.ToString(), out float result)) return false;
                else
                {
                    if (result <= 1)
                    {
                        PlayerDevice.Volume = result;
                        return true;
                    }
                    else return false;
                }
            };
            return true;
        }
    }
}