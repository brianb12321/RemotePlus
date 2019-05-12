using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension
{
    public class NodeCoreExtensionLibraryAttribute : ExtensionLibraryAttribute
    {
        public NodeCoreExtensionLibraryAttribute(Type startup, string name) : base(startup, name)
        {
        }
    }
}