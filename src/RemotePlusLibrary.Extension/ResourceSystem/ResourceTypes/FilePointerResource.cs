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
    public class FilePointerResource : FileResource
    {
        public FilePointerResource(string fileName, string id) : base(id, fileName)
        {
        }
        [DataMember]
        public override string ResourceType => "FilePointer";
        [DataMember]

        public override bool SaveToFile { get; set; } = true;

        public override void BeginIO()
        {
            
        }

        public override void Close()
        {
            
        }

        public override Stream OpenReadStream()
        {
            return new FileStream(FileName, FileMode.Open, FileAccess.Read);
        }

        public override Stream OpenWriteStream()
        {
            return new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.Write);
        }

        public override int Read(byte[] buffer, int offset, int length)
        {
            return new FileStream(FileName, FileMode.Open, FileAccess.Read).Read(buffer, offset, length);
        }

        public override string ToString()
        {
            return FileName;
        }

        public override void Write(byte[] data, int offset, int length)
        {
            new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.Write).Write(data, offset, length);
        }
    }
}