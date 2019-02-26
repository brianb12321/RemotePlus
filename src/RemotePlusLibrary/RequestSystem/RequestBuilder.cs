using RemotePlusLibrary.Core;
using RemotePlusLibrary.RequestSystem.DefaultRequestBuilders;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Windows.Forms;

namespace RemotePlusLibrary.RequestSystem
{
    /// <summary>
    /// Represents a request for information object.
    /// </summary>
    [DataContract]
    public abstract class RequestBuilder
    {
        /// <summary>
        /// The unique request interface (URI) string used for identifying a request interface to use.
        /// </summary>
        [DataMember]
        public string Interface { get; private set; }
        /// <summary>
        /// Sets how the client should acquire the data and provides settings for if there was en error during hhe process.
        /// </summary>
        [DataMember]
        public AcquisitionMode AcqMode { get; set; } = AcquisitionMode.None;
        [DataMember]
        public object Data { get; private set; }
        [DataMember]
        public Guid RequestingServer { get; set; }

        public RequestBuilder(string i)
        {
            Interface = i;
        }
    }
}