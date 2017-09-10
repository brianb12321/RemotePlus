using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.CommandSystem.CommandClasses
{
    /// <summary>
    /// Defines the input and output of a command.
    /// </summary>
    [DataContract]
    public class CommandRoutine
    {
        [DataMember]
        public CommandRequest Input { get; set; }
        [DataMember]
        public CommandResponse Output { get; set; }
        public CommandRoutine(CommandRequest input, CommandResponse output)
        {
            Input = input;
            Output = output;
        }
    }
}
