using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Synthesis;
using System.Runtime.Serialization;

namespace RemotePlusLibrary.Extension.ResourceSystem.ResourceTypes.Devices
{
    /// <summary>
    /// Provides a virtual device that will speak any data written to it.
    /// </summary>
    [DataContract]
    [Serializable]
    public class TTSDevice : IODevice
    {
        [DataMember]
        public int Rate
        {
            get
            {
                return int.Parse(DeviceProperties["Rate"].ToString());
            }
            set
            {
                DeviceProperties["Rate"].SetValue(value);
            }
        }
        public TTSDevice() : base("tts", "Text To Speech Device", "WinTTS Synth VDevice", Guid.NewGuid().ToString())
        {
        }

        public override void Close()
        {

        }
        public override bool Init()
        {
            base.Init();
            DeviceProperties.Add("Rate", new DeviceProperty(true, "Rate", 6));
            DeviceProperties["Rate"].PropertyChanged += (sender, e) =>
            {
                if (int.TryParse(e.ToString(), out int result)) return true;
                else { return false; }
            };
            return true;
        }
        public override string ResourceType => "TTS Device";

        public override Stream OpenReadStream()
        {
            throw new Exception("Can't open TTS device for reading.");
        }

        public override Stream OpenWriteStream()
        {
            return new TTSStream(Rate);
        }

        public override int Read(byte[] buffer, int offset, int length)
        {
            throw new Exception("Can't open TTS device for writing.");
        }

        public override void Write(byte[] data, int offset, int length)
        {
            new TTSStream(Rate).Write(data, offset, length);
        }
    }
    public class TTSStream : Stream
    {
        public override bool CanRead => false;
        public override bool CanSeek => true;
        public override bool CanWrite => true;
        public override long Length { get; }
        public override long Position { get; set; }
        private SpeechSynthesizer synth = new SpeechSynthesizer();
        public int Rate { get; set; }
        public TTSStream(int rate)
        {
            Rate = rate;
        }
        public override void Flush()
        {
            
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return 0;
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            synth.Rate = Rate;
            synth.Speak(Encoding.ASCII.GetString(buffer));
        }
    }
}