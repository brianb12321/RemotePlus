using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Discovery.Events
{
    [DataContract]
    public class ServerAddedEvent : ServerEvent
    {
        public ServerAddedEvent(Guid serverGuid, object sender) : base(serverGuid, sender)
        {
            ServerGuid = serverGuid;
        }
    }
}