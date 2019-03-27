using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Core
{
    /// <summary>
    /// The basic type of a client.
    /// </summary>
    [DataContract]
    [Flags]
    public enum ClientType
    {
        /// <summary>
        /// The client is a command-line client.
        /// </summary>
        [EnumMember]
        CommandLine = 1,
        /// <summary>
        /// The client is a GUI client.
        /// </summary>
        [EnumMember]
        GUI = 2,
        /// <summary>
        /// The client does not have an interface.
        /// </summary>
        [EnumMember]
        Headless = 4,
        /// <summary>
        /// The client is a server in a proxy environment.
        /// </summary>
        [EnumMember]
        Server = 8
    }
}
