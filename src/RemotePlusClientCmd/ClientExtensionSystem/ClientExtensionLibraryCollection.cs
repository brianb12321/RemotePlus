using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BetterLogger;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.Extension.ExtensionLoader;
using RemotePlusLibrary.Extension.ExtensionLoader.Initialization;

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

        public override void LoadExtension(string path, Action<string, LogLevel> callback, IInitEnvironment env)
        {
            throw new NotImplementedException();
        }
    }
}
