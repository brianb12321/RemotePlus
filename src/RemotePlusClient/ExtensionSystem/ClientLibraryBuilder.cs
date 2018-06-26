using RemotePlusLibrary.Extension.ExtensionLoader;
using RemotePlusLibrary.Extension.ExtensionLoader.Initialization;
using System.Collections.Generic;

namespace RemotePlusClient.ExtensionSystem
{
    internal class ClientLibraryBuilder : ILibraryBuilder
    {
        public Dictionary<string, List<ServerHook>> Hooks { get; private set; }

        public ClientLibraryBuilder(string name, string friendlyName, string version, ExtensionLibraryType libraryType)
        {
            Name = name;
            FriendlyName = friendlyName;
            Version = version;
            LibraryType = libraryType;
            Hooks = new Dictionary<string, List<ServerHook>>();
        }

        public string FriendlyName { get; private set; }

        public string Name { get; private set; }

        public string Version { get; private set; }

        public ExtensionLibraryType LibraryType { get; private set; }

        public void RegisterHook(string hookCategory, ServerHook hook)
        {
            Hooks[hookCategory].Add(hook);
        }
    }
}