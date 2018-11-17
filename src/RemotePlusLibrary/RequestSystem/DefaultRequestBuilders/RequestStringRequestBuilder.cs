using RemotePlusLibrary.RequestSystem.DefaultRequestBuilders.BaseBuilders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.RequestSystem.DefaultRequestBuilders
{
    [DataContract]
    public class RequestStringRequestBuilder : PromptRequestBuilder
    {
        public RequestStringRequestBuilder() : base("r_string")
        {
        }
    }
}
