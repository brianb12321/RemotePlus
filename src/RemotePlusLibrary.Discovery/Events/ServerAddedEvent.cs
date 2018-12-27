using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Discovery.Events
{
    [DataContract]
    public class ServerAddedEvent : TinyMessenger.TinyMessageBase
    {
        [DataMember]
        public Guid ServerGuid { get; set; }
        public ServerAddedEvent(Guid serverGuid, object sender) : base(sender)
        {
            ServerGuid = serverGuid;
        }
    }
}