using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.ResourceSystem
{
    [DataContract]
    [Serializable]
    public abstract class Resource
    {
        [DataMember]
        public string ResourceIdentifier { get; set; }
        public abstract override string ToString();
        protected Resource(string id)
        {
            ResourceIdentifier = id;
        }
    }
}