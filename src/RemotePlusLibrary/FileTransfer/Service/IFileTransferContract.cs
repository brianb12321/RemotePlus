using RemotePlusLibrary.FileTransfer.Service.PackageSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.FileTransfer.Service
{
    [ServiceContract]
    public interface IFileTransferContract
    {
        [OperationContract]
        RemoteFileInfo DownloadFile(DownloadRequest request);
        [OperationContract]
        void UploadFile(RemoteFileInfo request);
        [OperationContract]
        void DeleteFile(string remoteFile);
        [OperationContract]
        string[] GetPolicyObjectNames();
        [OperationContract]
        void SendFile(RemoteFileInfo fileRequest);
        [OperationContract]
        void SendFileUnrouted(RemoteFileInfo fileRequest);
    }
}