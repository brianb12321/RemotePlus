using System;
using System.Reflection;

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
