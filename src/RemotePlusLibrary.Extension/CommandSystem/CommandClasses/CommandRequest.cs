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
        public TokenSet Arguments { get; set; }
        public CommandRequest(CommandToken[] args)
        {
            Arguments = new TokenSet();
            foreach(CommandToken token in args)
            {
                Arguments.Add(token);
            }
        }
        public override string ToString()
        {
            return Arguments[0].Value;
        }
        public string GetFullCommand()
        {
            StringBuilder sb = new StringBuilder();
            foreach(CommandToken a in Arguments)
            {
                sb.Append(a.Value);
            }
            return sb.ToString();
        }
        public string GetBody()
        {
            StringBuilder sb = new StringBuilder();
            foreach (CommandToken a in Arguments.Skip(1))
            {
                sb.Append(a.Value);
            }
            return sb.ToString();
        }
    }
}