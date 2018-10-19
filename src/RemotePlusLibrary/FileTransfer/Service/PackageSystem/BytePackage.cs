using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.FileTransfer.Service.PackageSystem
{
    [DataContract]
    public class BytePackage : StreamPackage
    {
        [DataMember]
        public byte[] PackageContent { get; private set; }
        public BytePackage(byte[] content, long length)
        {
            Length = length;
            PackageContent = content;
            Data = new System.IO.MemoryStream(PackageContent, 0, (int)Length);
            Data.Seek(0, System.IO.SeekOrigin.Begin);
        }
    }
}