using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary
{
    /// <summary>
    /// Specifies how the server requests information from the client. One way is how the server handles errors like the user canceling the request.
    /// </summary>
    [DataContract]
    public enum AcquisitionMode
    {
        /// <summary>
        /// Throw an exception, if the user cancels out of the request.
        /// </summary>
        [EnumMember]
        ThrowIfCancel,
        /// <summary>
        /// Throw an exception, if the request does not exist.
        /// </summary>
        [EnumMember]
        ThrowIfNotFound,
        /// <summary>
        /// Does not specify any settings.
        /// </summary>
        [EnumMember]
        None, 
        /// <summary>
        /// If an exception occures, re-throw the exception.
        /// </summary>
        [EnumMember]
        ThrowIfException
    }
}
