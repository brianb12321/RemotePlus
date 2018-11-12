using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.RequestSystem.DefaultRequestOptions
{
    [DataContract]
    public class FileDialogRequestOptions
    {
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string Filter { get; set; }
    }
}
