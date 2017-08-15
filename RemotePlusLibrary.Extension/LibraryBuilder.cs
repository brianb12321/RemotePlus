using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension
{
    public class LibraryBuilder
    {
        public string Name { get; private set; }
        public string Version { get; private set; }
        public ExtensionLibraryType LibraryType { get; private set; }
        public LibraryBuilder(string n, string v, ExtensionLibraryType lt)
        {
            Name = n;
            Version = v;
            LibraryType = lt;
        }
    }
}
