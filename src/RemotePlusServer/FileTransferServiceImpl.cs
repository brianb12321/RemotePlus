using RemotePlusLibrary.Core;
using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.Extension.ResourceSystem;
using RemotePlusLibrary.Extension.ResourceSystem.ResourceTypes;
using RemotePlusLibrary.FileTransfer.Service;
using RemotePlusServer.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusServer
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall,
        IncludeExceptionDetailInFaults = true)]
    [GlobalExceptionIOCAttribute]
    public class FileTransferServiceImpl : IFileTransferContract
    {
        public void DeleteFile(string remoteFile)
        {
            File.Delete(remoteFile);
        }

        public RemoteFileInfo DownloadFile(DownloadRequest request)
        {
            RemoteFileInfo result = new RemoteFileInfo();
            string filePath = request.DownloadFileName;
            try
            {
                FileInfo fileInfo = new FileInfo(filePath);
                if (!fileInfo.Exists)
                {
                    throw new FileNotFoundException("File Not Found", filePath);
                }
                FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                result.FileName = Path.GetFileName(request.DownloadFileName);
                result.Length = fileInfo.Length;
                result.FileByteStream = stream;
                result.RemoteFilePath = request.DownloadFileName;
            }
            catch
            {
                throw;
            }
            return result;
        }

        public string[] GetPolicyObjectNames()
        {
            List<string> l = new List<string>();
            foreach (string f in Directory.GetFiles("policyObjects", "*.pobj", SearchOption.TopDirectoryOnly))
            {
                l.Add(f);
            }
            return l.ToArray();
        }
        Stream CopyData(RemoteFileInfo fileRequest)
        {
            Stream targetStream = new MemoryStream();
            Stream sourceStream = fileRequest.FileByteStream;
            const int bufferLength = 65000;
            byte[] buffer = new byte[bufferLength];
            int count = 0;
            while ((count = sourceStream.Read(buffer, 0, bufferLength)) > 0)
            {
                targetStream.Write(buffer, 0, count);
            }
            return targetStream;
        }
        public void SendFile(RemoteFileInfo fileRequest)
        {
            MemoryResource file = new MemoryResource(Path.GetFileNameWithoutExtension(fileRequest.FileName), fileRequest.FileName);
            file.Length = fileRequest.Length;
            file.Data = CopyData(fileRequest);
            file.Data.Seek(0, SeekOrigin.Begin);
            IOCContainer.GetService<IResourceManager>().AddResource(file);
            fileRequest.Dispose();
        }

        public void SendFileUnrouted(RemoteFileInfo fileRequest)
        {
            MemoryResource file = new MemoryResource(Path.GetFileNameWithoutExtension(fileRequest.FileName), fileRequest.FileName);
            file.FileName = fileRequest.FileName;
            file.Length = fileRequest.Length;
            file.Data = CopyData(fileRequest);
            file.Data.Seek(0, SeekOrigin.Begin);
            IOCContainer.GetService<IResourceManager>().AddResource(file);
            fileRequest.Dispose();
        }

        public void UploadFile(RemoteFileInfo request)
        {
            FileStream targetStream = null;
            Stream sourceStream = request.FileByteStream;
            string filePath = Path.Combine(request.RemoteFilePath, request.FileName);
            using (targetStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                const int bufferLength = 65000;
                byte[] buffer = new byte[bufferLength];
                int count = 0;
                while ((count = sourceStream.Read(buffer, 0, bufferLength)) > 0)
                {
                    targetStream.Write(buffer, 0, count);
                }
                targetStream.Close();
                sourceStream.Close();
            }
        }
    }
}