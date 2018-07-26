using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.Extension.ExtensionLoader;

namespace RemotePlusClientCmd.ClientExtensionSystem
{
    public class ClientExtensionLibraryCollection : ExtensionLibraryCollectionBase<ClientExtensionLibrary, CommandDelegate>
    {
        public ClientExtensionLibraryCollection()
        {
        }

        public override Dictionary<string, CommandDelegate> GetAllExtensions()
        {
            Dictionary<string, CommandDelegate> _allCommands = new Dictionary<string, CommandDelegate>();
            foreach(ClientExtensionLibrary cel in Libraries.Values)
            {
                foreach(KeyValuePair<string, CommandDelegate> command in cel.Extensions)
                {
                    _allCommands.Add(command.Key, command.Value);
                }
            }
            return _allCommands;
        }
    }
}
