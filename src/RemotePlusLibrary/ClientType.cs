﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary
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
        GUI
    }
}