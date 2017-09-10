using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.CommandSystem.CommandClasses
{
    [DataContract]
    public class CommandResponse
    {
        [DataMember]
        public int ResponseCode { get; set; }
        [DataMember]
        public string CustomStatusMessage { get; set; }
        [DataMember]
        public Dictionary<string, string> Metadata { get; set; }
        public CommandResponse(int code)
        {
            Metadata = new Dictionary<string, string>();
            ResponseCode = code;
        }
    }
}
