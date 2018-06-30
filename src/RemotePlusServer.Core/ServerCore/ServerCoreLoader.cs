using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Logging;
using RemotePlusLibrary.Extension;
using RemotePlusLibrary.Extension.ExtensionLoader;
using RemotePlusLibrary.Extension.ExtensionLoader.Initialization;
using RemotePlusServer.Core.ExtensionSystem;

namespace RemotePlusServer.Core.ServerCore
{
    public static class ServerCoreLoader
    {
        public static IServerCoreStartup LoadServerCoreLibrary(string FileName)
        {
            Assembly a = Assembly.LoadFrom(FileName);
            ServerCoreLibraryAttribute ea = a.GetCustomAttribute<ServerCoreLibraryAttribute>();
            if (ea != null)
            {
                if (!typeof(IServerCoreStartup).IsAssignableFrom(ea.Startup))
                {
                    throw new ArgumentException($"The startup type does not implement {nameof(IServerCoreStartup)}. A server core extension must have this implemented.");
                }
                else
                {
                    var st = (IServerCoreStartup)Activator.CreateInstance(ea.Startup);
                    return st;
                }
            }
            else
            {
                return null;
            }
        }
    }
}
