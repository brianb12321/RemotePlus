using RemotePlusLibrary.Core.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.ResourceSystem
{
    [DataContract]
    public class ResourceQuery
    {
        [DataMember]
        public string ResourceIdentifier { get; set; }
        [DataMember]
        public Guid Node { get; set; }
        public ResourceQuery(string id, Guid node)
        {
            ResourceIdentifier = id;
            Node = node;
        }
        public override string ToString()
        {
            return IOCContainer.GetService<IResourceManager>().GetResource<Resource>(this).ToString();
        }
    }
}