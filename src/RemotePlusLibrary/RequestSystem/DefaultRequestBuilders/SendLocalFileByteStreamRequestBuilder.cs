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
        [DataMember]
        public string FriendlyName { get; set; }
        public SendLocalFileByteStreamRequestBuilder(string friendlyName, string fileName) : base("global_sendByteStreamFilePackage")
        {
            FileName = fileName;
            FriendlyName = friendlyName;
        }
    }
}