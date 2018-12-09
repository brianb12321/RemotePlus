using System.IO;
using System.Runtime.Serialization;

namespace RemotePlusLibrary.Extension.ResourceSystem.ResourceTypes
{
    [DataContract]
    public class MemoryResource : Resource
    {
        [DataMember]
        public string FileName { get; set; }
        [IgnoreDataMember]
        public long Length { get; set; }
        [IgnoreDataMember]
        public Stream Data { get; set; } = new MemoryStream();
        public MemoryResource(string id, string fn) : base(id)
        {
            FileName = fn;
        }
        public void Close()
        {
            Data.Close();
        }
        public override string ToString()
        {
            return FileName;
        }
    }
}