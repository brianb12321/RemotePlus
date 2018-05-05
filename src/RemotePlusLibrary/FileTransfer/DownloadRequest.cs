using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.FileTransfer
{
    [MessageContract]
    public class DownloadRequest
    {
        [MessageBodyMember]
        public string DownloadFileName { get; set; }
    }
}
