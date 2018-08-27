using BetterLogger;
using RemotePlusLibrary.Extension;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.Extension.ExtensionLoader;
using RemotePlusLibrary.Extension.ExtensionLoader.Initialization;
using System;
using System.IO;
using System.Reflection;

namespace RemotePlusClientCmd.ClientExtensionSystem
{
    public class ClientExtensionLibrary : ExtensionLibraryBase<CommandDelegate>
    {
        protected ClientExtensionLibrary(Assembly assembly, string friendlyName, string name, ExtensionLibraryType type, Guid g, RequiresDependencyAttribute[] deps, Version v) : base(assembly, friendlyName, name, type, g, deps, v)
        {
        }

        public static ClientExtensionLibrary LoadClientLibrary(string fileName, Action<string, LogLevel> logCallback, IInitEnvironment env)
        {
            Assembly a = Assembly.LoadFrom(fileName);
            ClientExtensionLibrary lib;
            ExtensionLibraryAttribute ea = a.GetCustomAttribute<ExtensionLibraryAttribute>();
            if (ea != null)
            {
                if (ea.LibraryType == ExtensionLibraryType.Client || ea.LibraryType == ExtensionLibraryType.Both)
                {
                    Guid guid = Guid.Empty;
                    try
                    {
                        guid = ParseGuid(ea.Guid);
                    }
                    catch (ArgumentException)
                    {
                        throw;
                    }
                    Version version = ParseVersion(ea.Version);
                    var deps = LoadClientDependencies(a, logCallback, env);
                    if (!typeof(ILibraryStartup).IsAssignableFrom(ea.Startup))
                    {
                        throw new ArgumentException("The startup type does not implement ILibraryStartup.");
                    }
                    else
                    {
                        var st = (ILibraryStartup)Activator.CreateInstance(ea.Startup);
                        st.Init(new ClientLibraryBuilder(ea.Name, ea.FriendlyName, ea.Version, ea.LibraryType), env);
                        lib = new ClientExtensionLibrary(a, ea.FriendlyName, ea.Name, ea.LibraryType, guid, deps, version);
                        foreach (Type t in a.GetTypes())
                        {
                            if (t.IsClass == true && (typeof(CommandDelegate).IsAssignableFrom(t)))
                            {
                                var f = (CommandDelegate)Activator.CreateInstance(t);
                                lib.Extensions.Add(f.Method.Name, f);
                            }
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
        private static RequiresDependencyAttribute[] LoadClientDependencies(Assembly a, Action<string, LogLevel> logCallback, IInitEnvironment env)
        {
            logCallback($"Searching dependencies for {a.GetName().Name}", LogLevel.Info);
            RequiresDependencyAttribute[] deps = ExtensionLibraryBase<ClientExtensionLibrary>.FindDependencies(a);
            foreach (RequiresDependencyAttribute d in deps)
            {
                if (File.Exists(d.DependencyName))
                {
                    logCallback($"Found dependency {d.DependencyName}", LogLevel.Info);
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
                                    logCallback($"Loading dependency {d.DependencyName}", LogLevel.Info);
                                    LoadClientLibrary(d.DependencyName, logCallback, env);
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