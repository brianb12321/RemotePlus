using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.ResourceSystem.ResourceTypes.Devices
{
    public class NullDevice : IODevice
    {
        public override string ResourceType => "NullDevice";
        public NullDevice(string id) : base(id, "null", "Points to nothing.", Guid.Empty.ToString())
        {
        }

        public override void BeginIO()
        {
            
        }

        public override void Close()
        {
            
        }

        public override Stream OpenReadStream()
        {
            return Stream.Null;
        }

        public override Stream OpenWriteStream()
        {
            return Stream.Null;
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