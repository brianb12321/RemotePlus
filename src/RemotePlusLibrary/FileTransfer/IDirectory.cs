using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace RemotePlusLibrary.FileTransfer
{
    public interface IDirectory
    {
        [DataMember]
        string Name { get; set; }
        [DataMember]
        string FullName { get; set; }
        [DataMember]
        List<RemoteDirectory> Directories { get; set; }
        [DataMember]
        List<RemoteFile> Files { get; set; }
    }
}