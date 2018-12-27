using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ArduinoRemoteExtensions.Events
{
    [DataContract]
    public class ArduinoEvent : TinyMessenger.TinyMessageBase
    {
        [DataMember]
        public string Message { get; set; }
        public ArduinoEvent(string message, object sender) : base(sender)
        {
            Message = message;
        }
    }
}