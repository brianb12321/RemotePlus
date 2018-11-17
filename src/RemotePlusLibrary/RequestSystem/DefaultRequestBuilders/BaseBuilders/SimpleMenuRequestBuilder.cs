using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.RequestSystem.DefaultRequestBuilders.BaseBuilders
{
    [DataContract]
    public abstract class SimpleMenuRequestBuilder : PromptRequestBuilder
    {
        public SimpleMenuRequestBuilder(string i) : base(i)
        {
        }

        [DataMember]
        public Dictionary<string, string> MenuItems { get; set; } = new Dictionary<string, string>();
    }
}
