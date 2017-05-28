using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension
{
    [DataContract]
    public class ServerExtensionLibraryProgrammer : Programmer
    {
        [DataMember]
        public string FriendlyName { get; private set; }
        [DataMember]
        public string Name { get; private set; }
        [DataMember]
        public ExtensionLibraryType LibraryType { get; private set; }
        public ServerExtensionLibraryProgrammer(string friendlyName, string name, ExtensionLibraryType type)
        {
            FriendlyName = friendlyName;
            Name = name;
            LibraryType = type;
        }
        public override void Load(string file)
        {
            
        }

        public override void Save(string file)
        {
            
        }
    }
}
