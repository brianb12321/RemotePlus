using RemotePlusLibrary.Extension.ExtensionLoader;
using RemotePlusLibrary.Extension.ExtensionLoader.Initialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusServer.Core.ExtensionSystem
{
    public class ServerLibraryBuilder : ILibraryBuilder
    {
        public const string LOGIN_HOOK = "Login";
        public const string RUN_COMMAND_HOOK = "RunCommand";
        public ServerLibraryBuilder(string n, string fn, string v, ExtensionLibraryType lt)
        {
            Name = n;
            FriendlyName = fn;
            Version = v;
            LibraryType = lt;
            Hooks = new Dictionary<string, List<ServerHook>>();
            Hooks.Add(LOGIN_HOOK, new List<ServerHook>());
            Hooks.Add(RUN_COMMAND_HOOK, new List<ServerHook>());
        }

        public string Name { get; private set; }

        public string Version { get; private set; }

        public ExtensionLibraryType LibraryType { get; private set; }

        public string FriendlyName { get; private set; }
        public Dictionary<string, List<ServerHook>> Hooks { get; private set; }

        public void RegisterHook(string hookCategory, ServerHook hook)
        {
            Hooks[hookCategory].Add(hook);
        }
    }
}