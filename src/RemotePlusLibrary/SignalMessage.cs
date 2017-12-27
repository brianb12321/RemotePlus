using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary
{
    /// <summary>
    /// Represents a message that is deleivered to the client to signal of a server change, extension change, etc.
    /// </summary>
    [DataContract]
    public class SignalMessage
    {
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public string Value { get; set; }
        public SignalMessage(string message, string value)
        {
            Message = message;
            Value = value;
        }
    }
}