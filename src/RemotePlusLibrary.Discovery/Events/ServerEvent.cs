using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Discovery.Events
{
    /// <summary>
    /// Base class for all server events broadcasted by the proxy server.
    /// </summary>
    [DataContract]
    public abstract class ServerEvent : TinyMessenger.TinyMessageBase
    {
        [DataMember]
        public Guid ServerGuid { get; set; }
        protected ServerEvent(Guid serverGuid, object sender) : base(sender)
        {
            ServerGuid = serverGuid;
        }
    }
}