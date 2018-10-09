using RemotePlusLibrary.Extension.ExtensionLoader.Initialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.ExtensionLoader
{
    /// <summary>
    /// Represents an extension library that can be loaded into the system.
    /// </summary>
    /// <typeparam name="T">The extension that the library type will load</typeparam>
    public abstract class ExtensionLibraryBase
    {
        public List<RequiresDependencyAttribute> Dependencies { get; }
        public string FriendlyName { get; }
        public string Name { get; }
        public ExtensionLibraryType LibraryType { get; }
        public Guid Guid { get; }
        public Version Version { get; }
        private Assembly _assembly;
        protected ExtensionLibraryBase(Assembly assembly, string friendlyName, string name, ExtensionLibraryType type, Guid g, RequiresDependencyAttribute[] deps, Version v)
        {
            _assembly = assembly;
            FriendlyName = friendlyName;
            Name = name;
            LibraryType = type;
            Guid = g;
            Version = v;
            Dependencies = deps.ToList();
        }

        public static Guid ParseGuid(string guid)
        {
            if (string.IsNullOrEmpty(guid))
            {
                return Guid.NewGuid();
            }
            else
            {
                return Guid.Parse(guid);
            }
        }
        public static RequiresDependencyAttribute[] FindDependencies(Assembly file)
        {
            RequiresDependencyAttribute[] dep = file.GetCustomAttributes<RequiresDependencyAttribute>().ToArray();
            return dep;
        }
        public static Version ParseVersion(string version)
        {
            return Version.Parse(version);
        }
    }
}