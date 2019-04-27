using RemotePlusLibrary.SubSystem.Command.CommandClasses.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RemotePlusLibrary.SubSystem.Command.CommandClasses
{
    /// <summary>
    /// Encapuslates the command and parameters that will be sent to the server.
    /// </summary>
    [DataContract]
    public class CommandRequest
    {
        [DataMember]
        public ElementSet Arguments { get; set; }
        [DataMember]
        public CommandResponse LastCommand { get; set; }
        [DataMember]
        public bool HasLastCommand { get; set; }
        [IgnoreDataMember]
        public CancellationToken CancellationToken { get; private set; }
       
        public CommandRequest(ICommandElement[] args, CancellationToken ct)
        {
            CancellationToken = ct;
            Arguments = new ElementSet();
            foreach(ICommandElement token in args)
            {
                Arguments.Add(token);
            }
        }
        public override string ToString()
        {
            return Arguments[0].Value.ToString();
        }
        public string GetFullCommand()
        {
            StringBuilder sb = new StringBuilder();
            foreach(ICommandElement a in Arguments)
            {
                sb.Append(a);
            }
            return sb.ToString();
        }
        public string GetBody()
        {
            StringBuilder sb = new StringBuilder();
            foreach (ICommandElement a in Arguments.Skip(1))
            {
                sb.Append(a.Value);
            }
            return sb.ToString();
        }
    }
}