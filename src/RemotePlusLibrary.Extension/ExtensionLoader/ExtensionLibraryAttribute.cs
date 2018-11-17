using RemotePlusLibrary.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.ExtensionLoader
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class ExtensionLibraryAttribute : Attribute
    {
        public Type Startup { get; private set; }
        public NetworkSide LibraryType { get; set; }
        public string FriendlyName { get; set; }
        public string Name { get; private set; }
        public string Guid { get; set; }
        public string Version { get; set; } = "1.0.0.0";
        public ExtensionLibraryAttribute(Type startup, string name)
        {
            Startup = startup;
            Name = name;
        }
    }
}
