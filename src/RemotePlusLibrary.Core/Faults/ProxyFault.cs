using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Core.Faults
{
    [DataContract]
    public class ProxyFault
    {
        [DataMember]
        public Guid ServerGuid { get; set; } = Guid.Empty;
        public ProxyFault(Guid guid)
        {
            ServerGuid = guid;
        }
    }
}
