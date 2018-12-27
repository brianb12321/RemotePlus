using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Discovery.Events
{
    /// <summary>
    /// Use this event type to notify nodes of a generic message.
    /// </summary>
    [DataContract]
    public class ServerMessageEvent : ServerEvent
    {
        [DataMember]
        public string Message { get; set; }
        public ServerMessageEvent(Guid serverGuid, string message, object sender) : base(serverGuid, sender)
        {
            Message = message;
        }
    }
}