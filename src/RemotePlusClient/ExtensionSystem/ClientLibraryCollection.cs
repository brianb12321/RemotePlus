using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BetterLogger;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Extension;
using RemotePlusLibrary.Extension.ExtensionLoader;
using RemotePlusLibrary.Extension.ExtensionLoader.Initialization;

namespace RemotePlusClient.ExtensionSystem
{
    public class ClientLibraryCollection : ExtensionLibraryCollectionBase<ClientExtensionLibrary>
    {
        public Dictionary<string, IClientExtension> GetAllExtensions()
        {
            Dictionary<string, IClientExtension> d = new Dictionary<string, IClientExtension>();
            foreach(KeyValuePair<string, ClientExtensionLibrary> l in Libraries)
            {
                foreach(KeyValuePair<string, IClientExtension> e in l.Value.ClientExtensions)
                {
                    d.Add(e.Key, e.Value);
                }
            }
            return d;
        }
        private void internalLoadExtension(Assembly assembly, IInitEnvironment env)
        {
            //I assigned assembly to a to avoid breaking code.
            Assembly a = assembly;
            ClientExtensionLibrary lib;
            ExtensionLibraryAttribute ea = a.GetCustomAttribute<ExtensionLibraryAttribute>();
            if (ea != null)
            {
                if (ea.LibraryType.HasFlag(NetworkSide.Client))
                {
                    Guid guid = Guid.Empty;
                    try
                    {
                        guid = ClientExtensionLibrary.ParseGuid(ea.Guid);
                    }
                    catch (ArgumentException)
                    {
                        throw;
                    }
                    Version version = ClientExtensionLibrary.ParseVersion(ea.Version);
                    var deps = LoadClientDependencies(a, env);
                    if (!typeof(ILibraryStartup).IsAssignableFrom(ea.Startup))
                    {
                        throw new ArgumentException("The startup type does not implement ILibraryStartup.");
                    }
                    else
                    {
                        var st = (ILibraryStartup)Activator.CreateInstance(ea.Startup);
                        st.Init(new DefaultLibraryBuilder(ea.Name, ea.FriendlyName, ea.Version, ea.LibraryType), env);
                        lib = new ClientExtensionLibrary(a, ea.FriendlyName, ea.Name, ea.LibraryType, guid, deps, version);
                        foreach (Type t in a.GetTypes())
                        {
                            if (t.IsClass == true && (typeof(IClientExtension).IsAssignableFrom(t)))
                            {
                                var f = (IClientExtension)Activator.CreateInstance(t);
                                lib.ClientExtensions.Add(f.ExtensionName, f);
                                GlobalServices.Logger.Log($"Extension loaded: {f.ExtensionName}", LogLevel.Info);
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
            Libraries.Add(lib.Name, lib);
        }
        public override void LoadExtension(string path, IInitEnvironment env)
        {
            Assembly a = Assembly.LoadFrom(path);
            try
            {
                internalLoadExtension(a, env);
            }
            catch
            {
                throw;
            }
        }

        private RequiresDependencyAttribute[] LoadClientDependencies(Assembly a, IInitEnvironment env)
        {
            GlobalServices.Logger.Log($"Searching dependencies for {a.GetName().Name}", LogLevel.Info);
            RequiresDependencyAttribute[] deps = ClientExtensionLibrary.FindDependencies(a);
            foreach (RequiresDependencyAttribute d in deps)
            {
                if (File.Exists(d.DependencyName))
                {
                    GlobalServices.Logger.Log($"Found dependency {d.DependencyName}", LogLevel.Info);
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
                                    GlobalServices.Logger.Log($"Loading dependency {d.DependencyName}", LogLevel.Info);
                                    LoadExtension(d.DependencyName, env);
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
        public override void LoadExtension(byte[] data, IInitEnvironment env)
        {
            Assembly a = Assembly.Load(data);
            try
            {
                internalLoadExtension(a, env);
            }
            catch
            {
                throw;
            }
        }
    }
}