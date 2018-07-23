using System;
using System.IO;
using System.ServiceModel;

namespace RemotePlusLibrary.FileTransfer.Service
{
    public interface IFile : IDisposable
    {
        [MessageHeader(MustUnderstand = true)]
        string FileName { get; set; }
        [MessageHeader(MustUnderstand = true)]
        string RemoteFilePath { get; set; }
        [MessageHeader(MustUnderstand = true)]
        long Length { get; set; }
        [MessageBodyMember(Order = 1)]
        Stream FileByteStream { get; set; }
    }
}