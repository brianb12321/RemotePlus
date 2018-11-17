using RemotePlusLibrary.RequestSystem.DefaultRequestBuilders.BaseBuilders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.RequestSystem.DefaultRequestBuilders
{
    [DataContract]
    public class SendLocalFileByteStreamRequestBuilder : FileRequestBuilder
    {
        public SendLocalFileByteStreamRequestBuilder(string fileName) : base("global_sendByteStreamFilePackage")
        {
            FileName = fileName;
        }
    }
}