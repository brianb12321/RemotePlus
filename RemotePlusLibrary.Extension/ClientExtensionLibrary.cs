using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension
{
    public class ClientExtensionLibrary : ExtensionLibraryBase<IClientExtension>
    {
        private ClientExtensionLibrary(string friendlyName, string name, ExtensionLibraryType type) : base(friendlyName, name, type)
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
                    var st = (IClientLibraryStartup)Activator.CreateInstance(ea.Startup);
                    st.ClientInit();
                    lib = new ClientExtensionLibrary(ea.FriendlyName, ea.Name, ea.LibraryType);
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
