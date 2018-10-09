using System;
using System.Collections.Generic;
using System.Reflection;
using RemotePlusLibrary.Extension;
using RemotePlusLibrary.Extension.ExtensionLoader;

namespace RemotePlusClient.ExtensionSystem
{
    public class ClientExtensionLibrary : ExtensionLibraryBase
    {
        public Dictionary<string, IClientExtension> ClientExtensions { get; private set; }
        internal ClientExtensionLibrary(Assembly assembly, string friendlyName, string name, ExtensionLibraryType type, Guid g, RequiresDependencyAttribute[] deps, Version v) : base(assembly, friendlyName, name, type, g, deps, v)
        {
            ClientExtensions = new Dictionary<string, IClientExtension>();
        }
    }
}