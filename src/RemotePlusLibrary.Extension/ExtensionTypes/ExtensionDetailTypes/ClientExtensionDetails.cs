using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Extension;

namespace RemotePlusLibrary.Extension.ExtensionTypes.ExtensionDetailTypes
{
    [DataContract]
    public class ClientExtensionDetails : ExtensionDetails
    {
        public ClientExtensionDetails(string Name, string Version) : base(Name, Version)
        {
        }
    }
}
