using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.RequestSystem.DefaultRequestBuilders
{
    [DataContract]
    public class ConsoleReadLineRequestBuilder : BaseBuilders.PromptRequestBuilder
    {
        [DataMember]
        public ConsoleColor LineColor { get; set; }
        public ConsoleReadLineRequestBuilder(string message) : base("rcmd_readLine")
        {
            Message = message;
        }
    }
}