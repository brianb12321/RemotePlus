using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Discovery
{
    [DataContract]
    public class ProxyServerSetupSettings
    {
        [DataMember]
        public int DiscoveryPort { get; set; } = 9001;
        [DataMember]
        public string ProbeName { get; set; } = "Probe";
        [DataMember]
        public string AnnouncementName { get; set; } = "Announcement";
    }
}
