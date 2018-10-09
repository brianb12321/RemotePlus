using RemotePlusLibrary.Extension;
using RemotePlusLibrary.Extension.ExtensionLoader;
using System;
using System.Reflection;

namespace RemotePlusClientCmd.ClientExtensionSystem
{
    public class ClientExtensionLibrary : ExtensionLibraryBase
    {
        internal ClientExtensionLibrary(Assembly assembly, string friendlyName, string name, ExtensionLibraryType type, Guid g, RequiresDependencyAttribute[] deps, Version v) : base(assembly, friendlyName, name, type, g, deps, v)
        {
        }
    }
}