using RemotePlusLibrary.Extension.ResourceSystem.ResourceTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.SubSystem.Audio
{
    public abstract class AudioDevice : IODevice
    {
        public int Channels => (int)DeviceProperties["Channels"].Value;
        public AudioDevice(string id,
            string name,
            string description,
            string deviceID,
            int channels) : base(id, name, description, deviceID)
        {
            DeviceProperties.Add("Channels", new DeviceProperty(true, false, "Channels", channels));
        }

        public override abstract void Close();
        public override abstract Stream OpenReadStream();

        public override abstract Stream OpenWriteStream();

        public override abstract int Read(byte[] buffer, int offset, int length);

        public override abstract void Write(byte[] data, int offset, int length);
    }
}