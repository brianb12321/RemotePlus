using Logging;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace RemotePlusLibrary.Extension
{
    public class ServerExtensionLibrary : ExtensionLibraryBase<ServerExtension>
    {
        private ServerExtensionLibrary(string friendlyName, string name, ExtensionLibraryType type, Guid g) : base(friendlyName, name, type, g)
        {
        }

        public static ServerExtensionLibrary LoadServerLibrary(string FileName, Action<string, OutputLevel> Callback)
        {
            ServerExtensionLibrary lib;
            Assembly a = Assembly.LoadFrom(FileName);
            ExtensionLibraryAttribute ea = a.GetCustomAttribute<ExtensionLibraryAttribute>();
            if (ea != null)
            {
                if (ea.LibraryType == ExtensionLibraryType.Server || ea.LibraryType == ExtensionLibraryType.Both)
                {
                    if(!typeof(ILibraryStartup).IsAssignableFrom(ea.Startup))
                    {
                        throw new ArgumentException("The startup type does not implement ILibraryStartup.");
                    }
                    Guid guid = Guid.Empty;
                    try
                    {
                        guid = ParseGuid(ea.Guid);
                    }
                    catch (FormatException)
                    {
                        guid = Guid.NewGuid();
                        Callback($"Unable to parse GUID. Using random generated GUID. GUID: [{guid.ToString()}]", OutputLevel.Warning);
                    }
                    var st = (ILibraryStartup)Activator.CreateInstance(ea.Startup);
                    Callback("Beginning initialization.", Logging.OutputLevel.Info);
                    st.Init();
                    Callback("finished initalization.", Logging.OutputLevel.Info);
                    lib = new ServerExtensionLibrary(ea.FriendlyName, ea.Name, ea.LibraryType, guid);
                    foreach (Type t in a.GetTypes())
                    {
                        if (t.IsClass == true && (t.IsSubclassOf(typeof(ServerExtension))))
                        {
                            var e = (ServerExtension)Activator.CreateInstance(t);
                            lib.Extensions.Add(e.GeneralDetails.Name, e);
                            Callback($"Extension {e.GeneralDetails.Name} loaded.", Logging.OutputLevel.Info);
                        }
                    }
                }
                else
                {
                    throw new InvalidExtensionLibraryException($"The extension library {ea.FriendlyName} must be of type server.");
                }
            }
            else
            {
                throw new InvalidExtensionLibraryException("The library does not have an ExtensionLibraryAttrubte.");
            }
            return lib;
        }
    }
}