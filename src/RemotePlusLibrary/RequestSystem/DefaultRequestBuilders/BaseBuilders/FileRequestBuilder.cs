using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.RequestSystem.DefaultRequestBuilders.BaseBuilders
{
    [DataContract]
    public abstract class FileRequestBuilder : RequestBuilder
    {
        public FileRequestBuilder(string i) : base(i)
        {
        }

        [DataMember]
        public string FileName { get; set; }
    }
}
