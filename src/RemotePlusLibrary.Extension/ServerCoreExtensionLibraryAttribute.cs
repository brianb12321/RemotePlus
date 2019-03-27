using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension
{
    public class ServerCoreExtensionLibraryAttribute : ExtensionLibraryAttribute
    {
        public ServerCoreExtensionLibraryAttribute(Type startup, string name) : base(startup, name)
        {
        }
    }
}