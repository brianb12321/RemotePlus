using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.CommandSystem
{
    /// <summary>
    /// Specifies how a client should build the command-line prompt.
    /// </summary>
    [DataContract]
    public class PromptBuilder
    {
        [DataMember]
        public string Path { get; set; }
        [DataMember]
        public string AdditionalData { get; set; }
    }
}