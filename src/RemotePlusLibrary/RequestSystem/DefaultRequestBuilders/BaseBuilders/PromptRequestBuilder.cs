using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.RequestSystem.DefaultRequestBuilders.BaseBuilders
{
    [DataContract]
    public abstract class PromptRequestBuilder : RequestBuilder
    {
        public PromptRequestBuilder(string i) : base(i)
        {
        }

        [DataMember]
        public string Message { get; set; }
    }
}