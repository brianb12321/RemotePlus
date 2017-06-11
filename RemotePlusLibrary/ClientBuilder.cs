using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary
{
    [DataContract]
    public class ClientBuilder
    {
        [DataMember]
        public ClientType ClientType { get; private set; }
        [DataMember]
        public string FriendlyName { get; set; }
        [DataMember]
        public Dictionary<string, string> ExtraData { get; set; } = new Dictionary<string, string>();
        public ClientBuilder(ClientType ct)
        {
            ClientType = ct;
        }
    }
}
