using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.RequestSystem.DefaultRequestOptions
{
    [DataContract]
    public class PromptRequestOptions
    {
        [DataMember]
        public string Message { get; set; }
    }
}