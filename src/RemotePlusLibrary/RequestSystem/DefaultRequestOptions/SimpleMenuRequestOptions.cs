using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.RequestSystem.DefaultRequestOptions
{
    [DataContract]
    public class SimpleMenuRequestOptions : PromptRequestOptions
    {
        [DataMember]
        public Dictionary<string, string> MenuItems { get; set; } = new Dictionary<string, string>();
    }
}
