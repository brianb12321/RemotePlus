using RemotePlusLibrary.Discovery.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ArduinoRemoteExtensionsLib.Events
{
    [DataContract]
    public class ArduinoEvent : ServerEvent
    {
        [DataMember]
        public string Message { get; set; }
        public ArduinoEvent(Guid guid, string message, object sender) : base(guid, sender)
        {
            Message = message;
        }
    }
}