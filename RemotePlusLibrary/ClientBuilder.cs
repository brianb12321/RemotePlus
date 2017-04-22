using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary
{
    [DataContract]
    public class ClientBuilder
    {
        [DataMember]
        public string FriendlyName { get; set; }
        public Client Build(IRemoteClient Callback)
        {
            Client c = new Client();
            c.FriendlyName = FriendlyName;
            c.ClientCallback = Callback;
            return c;
        }
    }
}
