using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Extension.ExtensionLibraries;
using RemotePlusLibrary.Extension.ExtensionTypes;

namespace RemotePlusLibrary.Extension.LibraryCollections
{
    public class ServerExtensionLibraryCollection : ExtensionLibraryCollectionBase<ServerExtensionLibrary, ServerExtension>
    {
        public override Dictionary<string, ServerExtension> GetAllExtensions()
        {
            Dictionary<string, ServerExtension> s = new Dictionary<string, ServerExtension>();
            foreach(KeyValuePair<string, ServerExtensionLibrary> l in Libraries)
            {
                foreach(KeyValuePair<string, ServerExtension> e in l.Value.Extensions)
                {
                    s.Add(e.Key, e.Value);
                }
            }
            return s;
        }
    }
}
