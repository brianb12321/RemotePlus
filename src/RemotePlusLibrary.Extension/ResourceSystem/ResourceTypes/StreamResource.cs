using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.ResourceSystem.ResourceTypes
{
    [DataContract]
    [Serializable]
    public abstract class StreamResource : Resource
    {
        protected StreamResource(string id, string fileName) : base(id)
        {
            FileName = fileName;
        }

        [DataMember]
        public string FileName { get; set; }
        [IgnoreDataMember]
        [NonSerialized]
        private long _length;
        public long Length { get => _length; set => _length = value; }
        [IgnoreDataMember]
        [NonSerialized]
        private Stream _data;
        public Stream Data { get => _data; set => _data = value; }
        public override string ToString()
        {
            return FileName;
        }
        public abstract void Open();
        public abstract void Close();
    }
}