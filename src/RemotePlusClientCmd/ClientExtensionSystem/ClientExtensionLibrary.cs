﻿using RemotePlusLibrary.Core;
using RemotePlusLibrary.Extension;
using RemotePlusLibrary.Extension.ExtensionLoader;
using RemotePlusLibrary.Extension.ExtensionLoader.Initialization;
using System;
using System.Reflection;

namespace RemotePlusClientCmd.ClientExtensionSystem
{
    public class ClientExtensionLibrary : ExtensionLibraryBase
    {
        internal ClientExtensionLibrary(Assembly assembly, string friendlyName, string name, NetworkSide type, Guid g, RequiresDependencyAttribute[] deps, Version v, ILibraryStartup startup) : base(assembly, friendlyName, name, type, g, deps, v, startup)
        {
        }
    }
}