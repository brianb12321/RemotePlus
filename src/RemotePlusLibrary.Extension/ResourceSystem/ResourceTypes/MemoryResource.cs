using System;
using System.IO;
using System.Runtime.Serialization;

namespace RemotePlusLibrary.Extension.ResourceSystem.ResourceTypes
{
    [DataContract]
    [Serializable]
    public class MemoryResource : FileResource
    {

        [DataMember]
        public byte[] RawData { get; set; }
        [DataMember]
        public override string ResourceType => "Memory";
        [DataMember]

        public override bool SaveToFile { get; set; } = true;

        public MemoryResource(string id, string fn, byte[] rd) : base(id, fn)
        {
            RawData = rd;
        }

        public override void Close()
        {
            
        }

        public override Stream OpenReadStream()
        {
            return new MemoryStream(RawData, false);
        }

        public override Stream OpenWriteStream()
        {
            return new MemoryStream(RawData);
        }

        public override int Read(byte[] buffer, int offset, int length)
        {
            MemoryStream memStream = new MemoryStream(RawData);
            return memStream.Read(buffer, offset, length);
        }

        public override string ToString()
        {
            return FileName;
        }

        public override void Write(byte[] data, int offset, int length)
        {
            MemoryStream memStream = new MemoryStream(RawData);
            memStream.Write(data, offset, length);
        }

        public override void BeginIO()
        {
            
        }
    }
}