using System;
using System.IO;
using System.Runtime.Serialization;

namespace RemotePlusLibrary.Extension.ResourceSystem.ResourceTypes
{
    [DataContract]
    [Serializable]
    public class MemoryResource : StreamResource
    {

        [DataMember]
        public byte[] RawData { get; set; }
        public MemoryResource(string id, string fn, byte[] rd) : base(id, fn)
        {
            RawData = rd;
        }
        public override void Close()
        {
            Data.Close();
        }
        public override void Open()
        {
            Data = new MemoryStream(RawData);
            Data.Seek(0, SeekOrigin.Begin);
        }
    }
}