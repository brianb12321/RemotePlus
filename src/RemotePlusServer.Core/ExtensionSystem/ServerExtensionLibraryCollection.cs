using RemotePlusLibrary.Extension;
using RemotePlusLibrary.Extension.ExtensionLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusServer.Core.ExtensionSystem
{
    public class ServerExtensionLibraryCollection : ExtensionLibraryCollectionBase<ServerExtensionLibrary, string>
    {
        public override Dictionary<string, string> GetAllExtensions()
        {
            throw new NotImplementedException();
        }
    }
}
