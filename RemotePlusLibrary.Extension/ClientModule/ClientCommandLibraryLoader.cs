using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.ClientModule
{
    public class ClientCommandLibraryLoader
    {
        public static void LoadCommandLibrary(string fileName)
        {
            Assembly a = Assembly.LoadFrom(fileName);
            ExtensionLibraryAttribute ea = a.GetCustomAttribute<ExtensionLibraryAttribute>();
            if (ea != null)
            {
                if (ea.LibraryType == ExtensionLibraryType.Client || ea.LibraryType == ExtensionLibraryType.Both)
                {
                    if (!typeof(IClientCommandLibraryStartup).IsAssignableFrom(ea.Startup))
                    {
                        throw new ArgumentException("The startup type does not implement IClientModuleLibraryStartup");
                    }
                    else
                    {
                        var s = (IClientCommandLibraryStartup)Activator.CreateInstance(ea.Startup);
                        s.ModuleLibraryInit();
                    }
                }
                else
                {
                    throw new InvalidExtensionLibraryException($"The library {ea.FriendlyName} must be a client library or a server/client library.");
                }
            }
            else
            {
                throw new InvalidExtensionLibraryException("The client module library must have the ExtensionLibraryAttribute.");
            }
        }
    }
}
