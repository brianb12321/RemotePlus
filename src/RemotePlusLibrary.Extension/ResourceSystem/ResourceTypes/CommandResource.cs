using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.ResourceSystem.ResourceTypes
{
    [DataContract]
    public class CommandResource : Resource
    {
        public CommandResource(string id, string command) : base(id)
        {
            Command = command;
        }

        [DataMember]
        public override string ResourceType => "Command";
        [DataMember]
        public string Command { get; set; }
        public override bool SaveToFile { get; set; } = true;

        public override string ToString()
        {
            return Command;
        }
    }
}