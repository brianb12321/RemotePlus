using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemotePlusLibrary.Extension.ResourceSystem.ResourceTypes.Devices
{
    /// <summary>
    /// Represents the main input of a computer--the keyboard.
    /// </summary>
    public class KeyboardDevice : IODevice
    {
        public override string ResourceType => "Keyboard";
        public KeyboardDevice(string id, string name, string description, string deviceID) : base(id, name, description, deviceID)
        {
        }

        public override void Close()
        {
            
        }

        public override Stream OpenReadStream()
        {
            throw new Exception("Can't get keyboard read stream for reading.");
        }

        public override Stream OpenWriteStream()
        {
            return new KeyboardStream();
        }

        public override int Read(byte[] buffer, int offset, int length)
        {
            throw new Exception("Can't read from keyboard.");
        }

        public override void Write(byte[] data, int offset, int length)
        {
            new KeyboardStream().Write(data, offset, length);
        }
        private class KeyboardStream : Stream
        {
            public override bool CanRead => false;
            public override bool CanSeek => true;
            public override bool CanWrite => true;
            public override long Length { get; }
            public override long Position { get; set; }

            public override void Flush()
            {
                SendKeys.Flush();
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
                SendKeys.SendWait(Encoding.ASCII.GetString(buffer, offset, count));
            }
        }
    }
}