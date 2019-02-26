using RemotePlusLibrary.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusClient.CommonUI.Connection
{
    [DataContract]
    public class Connection
    {
        [DataMember]
        public string BaseAddress { get; set; }
        [DataMember]
        public int Port { get; set; }
        [IgnoreDataMember]
        public IRemote RemoteConnection { get; set; }
        [DataMember]
        public bool ConnectAsProxy { get; set; }
        [DataMember]
        public string WholeAddress { get; set; }
    }
}
