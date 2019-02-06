using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.ResourceSystem.ResourceTypes.Devices
{
    public class MouseDevice : IODevice
    {
        public MouseDevice(string id, string name, string description, string deviceID) : base(id, name, description, deviceID)
        {
        }
        public override string ResourceType => "Mouse";

        public override void BeginIO()
        {
            
        }

        public override void Close()
        {
            
        }

        public override Stream OpenReadStream()
        {
            throw new NotImplementedException("Cannot open mouse for reading.");
        }

        public override Stream OpenWriteStream()
        {
            throw new NotImplementedException("Cannot write to mouse.");
        }

        public override int Read(byte[] buffer, int offset, int length)
        {
            throw new NotImplementedException("Cannot open mouse for reading.");
        }

        public override void Write(byte[] data, int offset, int length)
        {
            throw new NotImplementedException("Cannot write to mouse."); ;
        }
    }
}
