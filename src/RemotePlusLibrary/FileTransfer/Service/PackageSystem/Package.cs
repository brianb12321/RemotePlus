using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.FileTransfer.Service.PackageSystem
{
    /// <summary>
    /// Represents a container that is transimitted to the server.
    /// </summary>
    [DataContract]
    public abstract class Package
    {
        /// <summary>
        /// The unique identifier for this package. This helps package receivers distinguish between multiple packages of the same kind.
        /// </summary>
        [DataMember]
        public string PackageHeader { get; set; }
    }
}