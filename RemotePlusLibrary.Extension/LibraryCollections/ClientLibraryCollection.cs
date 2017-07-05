using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary;
using RemotePlusLibrary.Extension.ExtensionLibraries;
using RemotePlusLibrary.Extension.ExtensionTypes;

namespace RemotePlusLibrary.Extension.LibraryCollections
{
    public class ClientLibraryCollection : ExtensionLibraryCollectionBase<ClientExtensionLibrary, IClientExtension>
    {
        public override Dictionary<string, IClientExtension> GetAllExtensions()
        {
            Dictionary<string, IClientExtension> d = new Dictionary<string, IClientExtension>();
            foreach(KeyValuePair<string, ClientExtensionLibrary> l in Libraries)
            {
                foreach(KeyValuePair<string, IClientExtension> e in l.Value.Extensions)
                {
                    d.Add(e.Key, e.Value);
                }
            }
            return d;
        }
    }
}
