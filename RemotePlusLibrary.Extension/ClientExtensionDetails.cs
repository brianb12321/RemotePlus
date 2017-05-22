using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension
{
    [DataContract]
    public class ClientExtensionDetails : ExtensionDetails
    {
        public ClientExtensionDetails(string Name, string Version) : base(Name, Version)
        {
        }
    }
}
