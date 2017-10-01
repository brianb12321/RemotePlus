using Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using RemotePlusLibrary.Extension;

namespace RemotePlusServer.ExtensionSystem
{
    /// <summary>
    /// Represents a server extension library.
    /// </summary>
    public class ServerExtensionLibrary : ExtensionLibraryBase<ServerExtension>
    {
        private ServerExtensionLibrary(string friendlyName, string name, ExtensionLibraryType type, Guid g, RequiresDependencyAttribute[] deps, Version v) : base(friendlyName, name, type, g, deps, v)
        {
        }

        public static ServerExtensionLibrary LoadServerLibrary(string FileName, Action<string, OutputLevel> callback, IInitEnvironment env)
        {
            ServerExtensionLibrary lib;
            Assembly a = Assembly.LoadFrom(FileName);
            ExtensionLibraryAttribute ea = a.GetCustomAttribute<ExtensionLibraryAttribute>();
            if (ea != null)
            {
                if (ea.LibraryType == ExtensionLibraryType.Server || ea.LibraryType == ExtensionLibraryType.Both)
                {
                    Guid guid = Guid.Empty;
                    try
                    {
                        guid = ParseGuid(ea.Guid);
                    }
                    catch (FormatException)
                    {
                        guid = Guid.NewGuid();
                        callback($"Unable to parse GUID. Using random generated GUID. GUID: [{guid.ToString()}]", OutputLevel.Warning);
                    }
                    Version version = ParseVersion(ea.Version);
                    var deps = LoadDependencies(a, callback, env);
                    if (!typeof(ILibraryStartup).IsAssignableFrom(ea.Startup))
                    {
                        throw new ArgumentException("The startup type does not implement ILibraryStartup.");
                    }
                    else
                    {
                        var st = (ILibraryStartup)Activator.CreateInstance(ea.Startup);
                        callback("Beginning initialization.", Logging.OutputLevel.Info);
                        st.Init(new LibraryBuilder(ea.Name, ea.FriendlyName, ea.Version, ea.LibraryType), env);
                        callback("finished initalization.", Logging.OutputLevel.Info);
                        lib = new ServerExtensionLibrary(ea.FriendlyName, ea.Name, ea.LibraryType, guid, deps, version);
                        foreach (Type t in a.GetTypes())
                        {
                            if (t.IsClass == true && (t.IsSubclassOf(typeof(ServerExtension))))
                            {
                                var e = (ServerExtension)Activator.CreateInstance(t);
                                lib.Extensions.Add(e.GeneralDetails.Name, e);
                                callback($"Extension {e.GeneralDetails.Name} loaded.", Logging.OutputLevel.Info);
                            }
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
        private static RequiresDependencyAttribute[] LoadDependencies(Assembly a, Action<string, OutputLevel> callback, IInitEnvironment env)
        {
            callback($"Searching dependencies for {a.GetName().Name}", OutputLevel.Info);
            RequiresDependencyAttribute[] deps = ExtensionLibraryBase<ServerExtensionLibrary>.FindDependencies(a);
            foreach (RequiresDependencyAttribute d in deps)
            {
                if(File.Exists(d.DependencyName))
                {
                    callback($"Found dependency {d.DependencyName}", OutputLevel.Info);
                    if (d.DependencyType != DependencyType.Resource)
                    {
                        try
                        {
                            Assembly da = Assembly.LoadFrom(d.DependencyName);
                            if(da.GetName().Version != d.Version)
                            {
                                throw new DependencyException($"Library {d.DependencyName}, version {da.GetName().Version} does not match requred version of {d.Version}");
                            }
                            else
                            {
                                if(d.LoadIfNotLoaded && d.DependencyType == DependencyType.RemotePlusLib)
                                {
                                    callback($"Loading dependency {d.DependencyName}", OutputLevel.Info);
                                    ServerExtensionLibrary.LoadServerLibrary(d.DependencyName, callback, env);
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