using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.CommandSystem
{
    /// <summary>
    /// Encapsulates a command that can be sent to the client.
    /// </summary>
    [DataContract]
    public class CommandDescription
    {
        [DataMember]
        public CommandHelpAttribute Help { get; set; }
        [DataMember]
        public CommandBehaviorAttribute Behavior { get; set; }
        [DataMember]
        public HelpPageAttribute HelpPage { get; set; }
        public string CommandName { get; set; }
    }
}
