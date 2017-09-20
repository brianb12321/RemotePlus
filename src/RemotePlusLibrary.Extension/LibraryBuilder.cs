using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension
{
    public class LibraryBuilder : ILibraryBuilder
    {

        public LibraryBuilder(string n, string fn, string v, ExtensionLibraryType lt)
        {
            Name = n;
            FriendlyName = fn;
            Version = v;
            LibraryType = lt;
        }

        public string Name { get; private set; }

        public string Version { get; private set; }

        public ExtensionLibraryType LibraryType { get; private set; }

        public string FriendlyName { get; private set; }
    }
}
