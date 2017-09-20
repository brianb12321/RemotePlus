using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.CommandSystem
{
    [AttributeUsage(AttributeTargets.Method)]
    [DataContract]
    public class HelpPageAttribute : Attribute
    {
        [DataMember]
        public string Details { get; private set; }
        [DataMember]
        public HelpSourceType Source { get; set; } = HelpSourceType.Text;
        public HelpPageAttribute(string details)
        {
            Details = details;
        }
    }
}
