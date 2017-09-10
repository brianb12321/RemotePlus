using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.CommandSystem.CommandClasses
{
    /// <summary>
    /// Encapuslates the command and parameters that will be sent to the server.
    /// </summary>
    [DataContract]
    public class CommandRequest
    {
        [DataMember]
        public string[] Arguments { get; set; }
        public CommandRequest(string[] args)
        {
            Arguments = args;
        }
        public override string ToString()
        {
            return Arguments[0];
        }
        public string GetFullCommand()
        {
            StringBuilder sb = new StringBuilder();
            foreach(string a in Arguments)
            {
                sb.Append(a);
            }
            return sb.ToString();
        }
    }
}
