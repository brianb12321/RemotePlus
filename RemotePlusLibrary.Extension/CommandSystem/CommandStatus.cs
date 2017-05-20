using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.CommandSystem
{
    [DataContract]
    public enum CommandStatus : int
    {
        [DataMember]
        Success = 0,
        [DataMember]
        Fail = -1
    }
}
