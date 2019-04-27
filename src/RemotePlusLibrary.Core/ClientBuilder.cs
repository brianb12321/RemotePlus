using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RemotePlusLibrary.Core
{
    /// <summary>
    /// Provides settings used to build the client.
    /// </summary>
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
