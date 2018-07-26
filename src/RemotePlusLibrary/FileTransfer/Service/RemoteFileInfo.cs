using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.FileTransfer.Service
{
    [MessageContract]
    public sealed class RemoteFileInfo : IFile
    {
        [MessageHeader]
        public string FileHeader { get; set; }
        [MessageHeader(MustUnderstand = true)]
        public string FileName { get; set; }
        [MessageHeader(MustUnderstand = true)]
        public string RemoteFilePath { get; set; }
        [MessageHeader(MustUnderstand = true)]
        public long Length { get; set; }
        [MessageBodyMember(Order = 1)]
        public Stream FileByteStream { get; set; }

        public void Dispose()
        {
            if(FileByteStream != null)
            {
                FileByteStream.Close();
                FileByteStream = null;
            }
            GC.SuppressFinalize(this);
        }
    }
}
