using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Extension.HookSystem;

namespace RemotePlusLibrary.Extension.ExtensionLoader.Initialization
{
    public delegate void ServerHook(HookArguments args);
    public interface ILibraryBuilder
    {
        string FriendlyName { get; }
        string Name { get; }
        string Version { get; }
        ExtensionLibraryType LibraryType { get; }
        void RegisterHook(string hookCategory, ServerHook hook);
    }
}