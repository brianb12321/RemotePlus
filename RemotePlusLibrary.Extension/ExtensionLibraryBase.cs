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
        public Guid Guid { get; private set; }
        protected ExtensionLibraryBase(string friendlyName, string name, ExtensionLibraryType type, Guid g)
        {
            FriendlyName = friendlyName;
            Name = name;
            LibraryType = type;
            Guid = g;
            Extensions = new Dictionary<string, T>();
        }

        public static Guid ParseGuid(string guid)
        {
            if (string.IsNullOrEmpty(guid))
            {
                return Guid.NewGuid();
            }
            else
            {
                return Guid.Parse(guid);
            }
        }
    }
}