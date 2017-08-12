using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Extension.ExtensionLibraries.LibraryStartupTypes;
using RemotePlusLibrary.Extension.ExtensionTypes;

namespace RemotePlusLibrary.Extension.ExtensionLibraries
{
    public class ClientExtensionLibrary : ExtensionLibraryBase<IClientExtension>
    {
        private ClientExtensionLibrary(string friendlyName, string name, ExtensionLibraryType type, Guid g) : base(friendlyName, name, type, g)
        {
        }
        public static ClientExtensionLibrary LoadClientLibrary(string fileName, Action<IClientExtension> Callback)
        {
            Assembly a = Assembly.LoadFrom(fileName);
            ClientExtensionLibrary lib;
            ExtensionLibraryAttribute ea = a.GetCustomAttribute<ExtensionLibraryAttribute>();
            if (ea != null)
            {
                if (ea.LibraryType == ExtensionLibraryType.Client || ea.LibraryType == ExtensionLibraryType.Both)
                {
                    if (!typeof(IClientLibraryStartup).IsAssignableFrom(ea.Startup))
                    {
                        throw new ArgumentException("The startup type does not implement ILibraryStartup.");
                    }
                    Guid guid = Guid.Empty;
                    try
                    {
                        guid = ParseGuid(ea.Guid);
                    }
                    catch (ArgumentException)
                    {
                        throw;
                    }
                    var st = (IClientLibraryStartup)Activator.CreateInstance(ea.Startup);
                    LibraryBuilder lb = new LibraryBuilder();
                    st.ClientInit(lb);
                    lib = new ClientExtensionLibrary(ea.FriendlyName, ea.Name, ea.LibraryType, guid);
                    foreach (Type t in a.GetTypes())
                    {
                        if (t.IsClass == true && (typeof(IClientExtension).IsAssignableFrom(t)))
                        {
                            var f = (IClientExtension)Activator.CreateInstance(t);
                            Callback(f);
                            lib.Extensions.Add(f.GeneralDetails.Name, f);
                        }
                    }
                }
                else
                {
                    throw new InvalidExtensionLibraryException($"The library {ea.FriendlyName} must be a client library or a server/client library.");
                }
            }
            else
            {
                throw new InvalidExtensionLibraryException("The client library must have the ExtensionLibraryAttribute.");
            }
            return lib;
        }
    }
}
