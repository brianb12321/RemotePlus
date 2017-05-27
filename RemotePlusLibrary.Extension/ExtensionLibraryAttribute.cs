using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class ExtensionLibraryAttribute : Attribute
    {
        public Type Startup { get; private set; }
        public ExtensionLibraryType LibraryType { get; set; }
        public string FriendlyName { get; set; }
        public string Name { get; private set; }
        public ExtensionLibraryAttribute(Type startup, string name)
        {
            Startup = startup;
            Name = name;
        }
    }
}
