using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using RemotePlusLibrary.FileTransfer.Service;
using RemotePlusLibrary.FileTransfer;
using System.IO;
using RemotePlusLibrary.Core;

namespace RemotePlusServer
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall,
        IncludeExceptionDetailInFaults = true)]
    [GlobalException(typeof(GlobalErrorHandler))]
    public class FileTransferService : IFileTransferContract
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
                if(!fileInfo.Exists)
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
