using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using RemotePlusLibrary.Extension;
using RemotePlusLibrary.Extension.ExtensionLoader;
using RemotePlusLibrary.Extension.ExtensionLoader.Initialization;
using BetterLogger;
using RemotePlusLibrary.Core;

namespace RemotePlusServer.Core.ExtensionSystem
{
    /// <summary>
    /// Represents a server extension library.
    /// </summary>
    public class ServerExtensionLibrary : ExtensionLibraryBase
    {
        internal ServerExtensionLibrary(Assembly assembly, string friendlyName, string name, NetworkSide type, Guid g, RequiresDependencyAttribute[] deps, Version v) : base(assembly, friendlyName, name, type, g, deps, v)
        {
        }
    }
}