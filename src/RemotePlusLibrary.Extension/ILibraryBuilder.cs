using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Extension.HookSystem;

namespace RemotePlusLibrary.Extension
{
    public delegate void ServerHook(HookArguments args);
    public interface ILibraryBuilder
    {
        Dictionary<string, List<ServerHook>> Hooks { get; }
        string FriendlyName { get; }
        string Name { get; }
        string Version { get; }
        ExtensionLibraryType LibraryType { get; }
        void RegisterHook(string hookCategory, ServerHook hook);
    }
}