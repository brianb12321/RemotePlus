using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Scripting
{
    [DataContract]
    public class ScriptGlobalInformation
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public ScriptGlobalType Type { get; set; }
        [DataMember]
        public List<ScriptGlobalInformation> Members { get; set; } = new List<ScriptGlobalInformation>();
    }
}
