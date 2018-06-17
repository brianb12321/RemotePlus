using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Discovery
{
    [DataContract]
    public class ProxyConnection
    {
        [DataMember]
        public string ProbeURL { get; set; } = "net.tcp://localhost:9001/Probe";
        [DataMember]
        public string AnnouncementURL { get; set; } = "net.tcp://localhost:9001/Announcement";
    }
}
