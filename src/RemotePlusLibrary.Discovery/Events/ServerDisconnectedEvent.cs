using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Discovery.Events
{
    [DataContract]
    public class ServerDisconnectedEvent : ServerEvent
    {
        [DataMember]
        public bool Faulted { get; set; }
        public ServerDisconnectedEvent(Guid serverGuid, bool faulted, object sender) : base(serverGuid, sender)
        {
            Faulted = faulted;
        }
    }
}