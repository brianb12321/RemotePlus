using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Configuration.ServerSettings
{
    [DataContract]
    public class ProxyConnection
    {
        [DataMember]
        public string ProxyServerURL { get; set; } = "net.tcp://localhost:9001/Proxy";
    }
}
