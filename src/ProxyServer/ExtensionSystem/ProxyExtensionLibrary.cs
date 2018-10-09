using System;
using RemotePlusLibrary.Extension.ExtensionLoader;
using System.Collections.Generic;
using RemotePlusLibrary.Extension;
using System.Reflection;

namespace ProxyServer.ExtensionSystem
{
    public class ProxyExtensionLibrary : ExtensionLibraryBase
    {
        public ProxyExtensionLibrary(Assembly assembly, string friendlyName, string name, ExtensionLibraryType type, Guid g, RequiresDependencyAttribute[] deps, Version v) : base(assembly, friendlyName, name, type, g, deps, v)
        {
        }
    }
}