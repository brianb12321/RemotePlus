using RemotePlusLibrary.Core;
using RemotePlusLibrary.FileTransfer;
using RemotePlusLibrary.FileTransfer.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusClient.CommonUI
{
    public class FileTransfer
    {
        string _baseAddress = "";
        int port = 0;
        public FileTransfer(string baseAddress, int p)
        {
            _baseAddress = baseAddress;
            port = p;
        }
        public void UploadFile(string localFile, string remotePath)
        {
            var connection = EstablishConnection();
            FileInfo file = new FileInfo(localFile);
            RemoteFileInfo uploadRequest = new RemoteFileInfo();
            using (FileStream stream = new FileStream(localFile, FileMode.Open, FileAccess.Read))
            {
                uploadRequest.FileName = Path.GetFileName(localFile);
                uploadRequest.Length = file.Length;
                uploadRequest.FileByteStream = stream;
                uploadRequest.RemoteFilePath = remotePath;
                connection.UploadFile(uploadRequest);
            }
        }
        public void DownloadFile(string fileToDownload, string newLocation)
        {
            var connection = EstablishConnection();
            DownloadRequest request = new DownloadRequest();
            request.DownloadFileName = fileToDownload;
            var downloadFile = connection.DownloadFile(request);
            byte[] buffer = new byte[65000];
            int bytesRead = 0;
            bytesRead = downloadFile.FileByteStream.Read(buffer, 0, buffer.Length);
            while(bytesRead > 0)
            {
                using (FileStream outputStream = new FileStream(newLocation, FileMode.Create, FileAccess.Write))
                {
                    outputStream.Write(buffer, 0, bytesRead);
                    outputStream.Flush();
                    buffer = new byte[65000];
                    bytesRead = downloadFile.FileByteStream.Read(buffer, 0, buffer.Length);
                    while(bytesRead > 0)
                    {
                        outputStream.Write(buffer, 0, bytesRead);
                        bytesRead = downloadFile.FileByteStream.Read(buffer, 0, 65000);
                    }
                }
            }
        }
        public void DeleteFile(string remoteFile)
        {
            var connection = EstablishConnection();
            connection.DeleteFile(remoteFile);
        }
        private IFileTransferContract EstablishConnection()
        {
            var binding = _ConnectionFactory.BuildBinding();
            binding.TransferMode = TransferMode.Streamed;
            ChannelFactory<IFileTransferContract> channel = new ChannelFactory<IFileTransferContract>(binding, new EndpointAddress($"net.tcp://{_baseAddress}:{port}/FileTransfer"));
            return channel.CreateChannel();
        }
        public string[] GetPolicyObjectNames()
        {
            var connection = EstablishConnection();
            return connection.GetPolicyObjectNames();
        }
    }
}