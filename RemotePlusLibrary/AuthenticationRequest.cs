using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary
{
    /// <summary>
    /// Provides information that the server will use to create a login screen.
    /// </summary>
    [DataContract]
    public class AuthenticationRequest
    {
        /// <summary>
        /// The reason of the login request.
        /// </summary>
        [DataMember]
        public string Reason { get; set; }
        /// <summary>
        /// The type or severity of the request.
        /// </summary>
        [DataMember]
        public AutehnticationSeverity Severity { get; set; }
        /// <summary>
        /// Creates a new instance of the AuthenticationRequest class.
        /// </summary>
        /// <param name="sev">The authentication severity</param>
        public AuthenticationRequest(AutehnticationSeverity sev)
        {
            Severity = sev;
        }
    }
}
