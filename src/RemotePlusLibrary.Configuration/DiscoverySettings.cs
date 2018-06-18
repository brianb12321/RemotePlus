using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Configuration
{
    [DataContract]
    public class DiscoverySettings
    {
        [DataMember]
        public ProxyConnectionMode DiscoveryBehavior { get; set; } = ProxyConnectionMode.None;
        [DataMember]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public ProxyServerSetupSettings Setup { get; set; } = new ProxyServerSetupSettings();
        [DataMember]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public ProxyConnection Connection { get; set; } = new ProxyConnection();
        [DataMember]
        public bool ConnectToBuiltInProxyServer { get; set; } = true;
    }
}
