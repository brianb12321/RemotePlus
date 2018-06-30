using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Extension.ExtensionLoader;

namespace RemotePlusServer.Core.ServerCore
{
    /// <summary>
    /// speicifes that the extension library is a server core extension.
    /// </summary>
    public class ServerCoreLibraryAttribute : ExtensionLibraryAttribute
    {
        public ServerCoreLibraryAttribute(Type startup, string name) : base(startup, name)
        {
        }
    }
}
