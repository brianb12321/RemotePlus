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
    public abstract class FileResource : IOResource
    {
        protected FileResource(string id, string fileName) : base(id)
        {
            FileName = fileName;
        }

        [DataMember]
        public string FileName { get; set; }
        public override string ResourceType { get; }

        public override abstract void Close();

        public override abstract Stream OpenReadStream();

        public override abstract Stream OpenWriteStream();

        public override abstract int Read(byte[] buffer, int offset, int length);

        public override string ToString()
        {
            return FileName;
        }

        public override abstract void Write(byte[] data, int offset, int length);
    }
}
