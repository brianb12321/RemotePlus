using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Extension.ExtensionLibraries.LibraryStartupTypes;
using RemotePlusLibrary.Extension.ExtensionTypes;
using Logging;
using System.IO;

namespace RemotePlusLibrary.Extension.ExtensionLibraries
{
    public class ClientExtensionLibrary : ExtensionLibraryBase<IClientExtension>
    {
        private ClientExtensionLibrary(string friendlyName, string name, ExtensionLibraryType type, Guid g, RequiresDependencyAttribute[] deps, Version v) : base(friendlyName, name, type, g, deps, v)
        {
        }
        public static ClientExtensionLibrary LoadClientLibrary(string fileName, Action<IClientExtension> callback, Action<string, OutputLevel> logCallback)
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
                    Version version = ExtensionLibraryBase<ClientExtensionLibrary>.ParseVersion(ea.Version);
                    var deps = LoadClientDependencies(a, logCallback, callback);
                    var st = (IClientLibraryStartup)Activator.CreateInstance(ea.Startup);
                    st.ClientInit(new LibraryBuilder(ea.Name, ea.FriendlyName, ea.Version, ea.LibraryType));
                    lib = new ClientExtensionLibrary(ea.FriendlyName, ea.Name, ea.LibraryType, guid, deps, version);
                    foreach (Type t in a.GetTypes())
                    {
                        if (t.IsClass == true && (typeof(IClientExtension).IsAssignableFrom(t)))
                        {
                            var f = (IClientExtension)Activator.CreateInstance(t);
                            callback(f);
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
        private static RequiresDependencyAttribute[] LoadClientDependencies(Assembly a, Action<string, OutputLevel> logCallback, Action<IClientExtension> callback)
        {
            logCallback($"Searching dependencies for {a.GetName().Name}", OutputLevel.Info);
            RequiresDependencyAttribute[] deps = ExtensionLibraryBase<ServerExtensionLibrary>.FindDependencies(a);
            foreach (RequiresDependencyAttribute d in deps)
            {
                if (File.Exists(d.DependencyName))
                {
                    logCallback($"Found dependency {d.DependencyName}", OutputLevel.Info);
                    if (d.DependencyType != DependencyType.Resource)
                    {
                        try
                        {
                            Assembly da = Assembly.LoadFrom(d.DependencyName);
                            if (da.GetName().Version != d.Version)
                            {
                                throw new DependencyException($"Library {d.DependencyName}, version {da.GetName().Version} does not match requred version of {d.Version}");
                            }
                            else
                            {
                                if (d.LoadIfNotLoaded && d.DependencyType == DependencyType.RemotePlusLib)
                                {
                                    logCallback($"Loading dependency {d.DependencyName}", OutputLevel.Info);
                                    ClientExtensionLibrary.LoadClientLibrary(d.DependencyName, callback, logCallback);
                                }
                            }
                        }
                        catch
                        {
                            throw;
                        }
                    }
                }
                else
                {
                    if (d.DependencyType == DependencyType.Resource)
                    {
                        throw new DependencyException($"Library {a.GetName().Name} is dependent on {d.DependencyName}");
                    }
                    else
                    {
                        throw new DependencyException($"Library {a.GetName().Name} is dependent on {d.DependencyName}, version {d.Version.ToString()}");
                    }
                }
            }
            return deps;
        }
    }
}
