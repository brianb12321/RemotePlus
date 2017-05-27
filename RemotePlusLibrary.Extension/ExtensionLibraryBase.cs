using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension
{
    public abstract class ExtensionLibraryBase<T>
    {
        public Dictionary<string, T> Extensions { get; private set; }
        public string FriendlyName { get; private set; }
        public string Name { get; private set; }
        public ExtensionLibraryType LibraryType { get; private set; }
        protected ExtensionLibraryBase(string friendlyName, string name, ExtensionLibraryType type)
        {
            FriendlyName = friendlyName;
            Name = name;
            LibraryType = type;
            Extensions = new Dictionary<string, T>();
        }
    }
}
