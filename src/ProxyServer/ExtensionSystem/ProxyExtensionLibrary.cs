using System;
using RemotePlusLibrary.Extension.ExtensionLoader;
using System.Collections.Generic;
using RemotePlusLibrary.Extension;
using System.Reflection;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Extension.ExtensionLoader.Initialization;

namespace ProxyServer.ExtensionSystem
{
    public class ProxyExtensionLibrary : ExtensionLibraryBase
    {
        public ProxyExtensionLibrary(Assembly assembly, string friendlyName, string name, NetworkSide type, Guid g, RequiresDependencyAttribute[] deps, Version v, ILibraryStartup startup) : base(assembly, friendlyName, name, type, g, deps, v, startup)
        {
        }
    }
}