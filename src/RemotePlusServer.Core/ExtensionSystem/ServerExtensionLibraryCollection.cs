using BetterLogger;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Extension;
using RemotePlusLibrary.Extension.ExtensionLoader;
using RemotePlusLibrary.Extension.ExtensionLoader.Initialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace RemotePlusServer.Core.ExtensionSystem
{
    public class ServerExtensionLibraryCollection : ExtensionLibraryCollectionBase<ServerExtensionLibrary>
    {
        const string EXTENSION_LOADER = "Extension Loader";
        private void internalLoadExtension(Assembly a, IInitEnvironment env)
        {
            ServerExtensionLibrary lib;
            ExtensionLibraryAttribute ea = a.GetCustomAttribute<ExtensionLibraryAttribute>();
            if (ea != null)
            {
                if (Libraries.ContainsKey(ea.Name))
                {
                    throw new InvalidExtensionLibraryException($"The extension library '{ea.Name}' is already in the system.");
                }
                if (ea.LibraryType.HasFlag(NetworkSide.Server))
                {
                    Guid guid = Guid.Empty;
                    try
                    {
                        guid = ExtensionLibraryBase.ParseGuid(ea.Guid);
                    }
                    catch (FormatException)
                    {
                        guid = Guid.NewGuid();
                        GlobalServices.Logger.Log($"Unable to parse GUID. Using random generated GUID. GUID: [{guid.ToString()}]", LogLevel.Warning);
                    }
                    Version version = ServerExtensionLibrary.ParseVersion(ea.Version);
                    var deps = LoadDependencies(a, env);
                    if (!typeof(ILibraryStartup).IsAssignableFrom(ea.Startup))
                    {
                        throw new ArgumentException("The startup type does not implement ILibraryStartup.");
                    }
                    else
                    {
                        var st = (ILibraryStartup)Activator.CreateInstance(ea.Startup);
                        GlobalServices.Logger.Log("Beginning initialization.", LogLevel.Info, EXTENSION_LOADER);
                        DefaultLibraryBuilder builder = new DefaultLibraryBuilder(ea.Name, ea.FriendlyName, ea.Version, ea.LibraryType);
                        st.Init(builder, env);
                        GlobalServices.Logger.Log("finished initialization.", LogLevel.Info, EXTENSION_LOADER);
                        lib = new ServerExtensionLibrary(a, ea.FriendlyName, ea.Name, ea.LibraryType, guid, deps, version);
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
        private RequiresDependencyAttribute[] LoadDependencies(Assembly a, IInitEnvironment env)
        {
            GlobalServices.Logger.Log($"Searching dependencies for {a.GetName().Name}", LogLevel.Info, EXTENSION_LOADER);
            RequiresDependencyAttribute[] deps = ExtensionLibraryBase.FindDependencies(a);
            foreach (RequiresDependencyAttribute d in deps)
            {
                if (File.Exists(d.DependencyName))
                {
                    GlobalServices.Logger.Log($"Found dependency {d.DependencyName}", LogLevel.Info, EXTENSION_LOADER);
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
                                    GlobalServices.Logger.Log($"Loading dependency {d.DependencyName}", LogLevel.Info, EXTENSION_LOADER);
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

        public static void LoadExtensionsInFolder()
        {
            List<string> excludedFiles = new List<string>();
            GlobalServices.Logger.Log("Loading extensions...", LogLevel.Info, EXTENSION_LOADER);
            if (Directory.Exists("extensions"))
            {
                if (File.Exists("extensions\\excludes.txt"))
                {
                    GlobalServices.Logger.Log("Found an excludes.txt file. Reading file...", LogLevel.Info, EXTENSION_LOADER);
                    foreach (string excludedFile in File.ReadLines("extensions\\excludes.txt"))
                    {
                        GlobalServices.Logger.Log($"{excludedFile} is excluded from the extension search.", LogLevel.Info, EXTENSION_LOADER);
                        excludedFiles.Add("extensions\\" + excludedFile);
                    }
                    GlobalServices.Logger.Log("Finished reading extension exclusion file.", LogLevel.Info, EXTENSION_LOADER);
                }
                ServerInitEnvironment env = new ServerInitEnvironment(false);
                foreach (string files in Directory.GetFiles("extensions"))
                {
                    if (Path.GetExtension(files) == ".dll" && !excludedFiles.Contains(files))
                    {
                        try
                        {
                            GlobalServices.Logger.Log($"Found extension file ({Path.GetFileName(files)})", LogLevel.Info, EXTENSION_LOADER);
                            env.PreviousError = GlobalServices.Logger.ErrorCount > 0;
                            ServerManager.DefaultCollection.LoadExtension(files, env);
                        }
                        catch (Exception ex)
                        {
                            GlobalServices.Logger.Log($"Could not load \"{files}\" because of a load error or initialization error. Error: {ex.Message}", LogLevel.Warning, EXTENSION_LOADER);
                        }
                        env.InitPosition++;
                    }
                }
                GlobalServices.Logger.Log($"{ServerManager.DefaultCollection.Libraries.Count} extension libraries loaded.", LogLevel.Info, EXTENSION_LOADER);
            }
            else
            {
                GlobalServices.Logger.Log("The extensions folder does not exist.", LogLevel.Info, EXTENSION_LOADER);
            }
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