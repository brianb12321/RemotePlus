using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Configuration.ServerSettings
{
    [DataContract]
    public class ProxyServerSetupSettings
    {
        [DataMember]
        public int DiscoveryPort { get; set; } = 9001;
        [DataMember]
        public string ProxyEndpointName { get; set; } = "Proxy";
        [DataMember]
        public string ProxyClientEndpointName { get; set; } = "ProxyClient";
    }
}
