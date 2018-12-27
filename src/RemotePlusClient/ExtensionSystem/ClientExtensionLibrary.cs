using System;
using System.Collections.Generic;
using System.Reflection;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Extension;
using RemotePlusLibrary.Extension.ExtensionLoader;
using RemotePlusLibrary.Extension.ExtensionLoader.Initialization;

namespace RemotePlusClient.ExtensionSystem
{
    public class ClientExtensionLibrary : ExtensionLibraryBase
    {
        public Dictionary<string, IClientExtension> ClientExtensions { get; private set; }
        internal ClientExtensionLibrary(Assembly assembly, string friendlyName, string name, NetworkSide type, Guid g, RequiresDependencyAttribute[] deps, Version v, ILibraryStartup startup) : base(assembly, friendlyName, name, type, g, deps, v, startup)
        {
            ClientExtensions = new Dictionary<string, IClientExtension>();
        }
    }
}