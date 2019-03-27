using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.SubSystem.Command
{
    [DataContract]
    public enum CommandStatus : int
    {
        [EnumMember]
        Success = 0,
        [EnumMember]
        Fail = -1,
        [EnumMember]
        AccessDenied = -2,
        [EnumMember]
        UnsupportedClient = -3,
        [EnumMember]
        InvalidTokens = 1
    }
}
