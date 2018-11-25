using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace RemotePlusLibrary.Core.Faults
{
    /// <summary>
    /// Global class for all server faults.
    /// </summary>
    [DataContract]
    public class ServerFault
    {
        [DataMember]
        public List<string> LoadedServerExtensionLibs { get; set; } = new List<string>();
        [DataMember]
        public string StackTrace { get; set; }
        public ServerFault(string stackTrace, List<string> libs)
        {
            LoadedServerExtensionLibs = libs;
            StackTrace = stackTrace;

        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(Environment.NewLine + "Server Fault Stack Trace");
            sb.AppendLine("=================");
            sb.AppendLine(StackTrace + Environment.NewLine);
            sb.AppendLine("Loaded extension libraries on the server:");
            sb.AppendLine("=========================================");
            foreach (string name in LoadedServerExtensionLibs)
            {
                sb.AppendLine(name);
            }
            return sb.ToString();
        }
    }
}
