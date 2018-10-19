using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.FileTransfer.Service.PackageSystem
{
    [DataContract]
    public class StreamPackage : Package, IDisposable
    {
        [DataMember]
        public long Length { get; set; }
        [DataMember]
        public Stream Data { get; set; } = new MemoryStream();
        [DataMember]
        public bool KeepAlive { get; set; }
        public void Dispose()
        {
            if (Data != null)
            {
                Data.Close();
                Data = null;
            }
            GC.SuppressFinalize(this);
        }
    }
}
