using RemotePlusLibrary.Core.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension
{
    public static class ServerCoreLoader
    {
        public static IServerCoreStartup LoadServerCore(Assembly a)
        {
            var attrib = a.GetCustomAttribute<ServerCoreExtensionLibraryAttribute>();
            if(attrib != null)
            {
                var startupType = a.GetTypes().Where(t => typeof(IServerCoreStartup).IsAssignableFrom(t)).FirstOrDefault();
                if (startupType != null)
                {
                    return (IServerCoreStartup)Activator.CreateInstance(startupType);
                }
                else return null;
            }
            return null;
        }
    }
}
