using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension
{
    [DataContract]
    public class ExtensionDetails
    {
        [DataMember]
        public string Name { get; private set; }
        [DataMember]
        public string Version { get; private set; }
        [DataMember]
        public string Description { get; set; }
        public ExtensionDetails(string Name, string Version)
        {
            this.Name = Name;
            this.Version = Version;
        }
    }
}
