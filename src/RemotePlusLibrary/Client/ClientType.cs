using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Client
{
    /// <summary>
    /// The basic type of a client.
    /// </summary>
    [DataContract]
    public enum ClientType
    {
        /// <summary>
        /// The client is a command-line client.
        /// </summary>
        [EnumMember]
        CommandLine,
        /// <summary>
        /// The client is a GUI client.
        /// </summary>
        [EnumMember]
        GUI,
        /// <summary>
        /// The client does not have an interface.
        /// </summary>
        [EnumMember]
        Headless,
        /// <summary>
        /// The client is a server in a proxy environment.
        /// </summary>
        [EnumMember]
        Server
    }
}
