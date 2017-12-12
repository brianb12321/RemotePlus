using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension
{
    [DataContract]
    public enum ExtensionDevelopmentState
    {
        [EnumMember] InDevelopment,
        [EnumMember] Obsolete,
        [EnumMember] Official
    }
}
