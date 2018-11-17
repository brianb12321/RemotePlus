using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.RequestSystem.DefaultRequestBuilders
{
    [DataContract]
    public class FileDialogRequestBuilder : RequestBuilder
    {
        public FileDialogRequestBuilder() : base("r_selectLocalFile")
        {
        }

        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string Filter { get; set; }
    }
}
