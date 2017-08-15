using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension
{
    public abstract class ExtensionLibraryBase<T>
    {
        public List<RequiresDependencyAttribute> Dependencies { get; private set; }
        public Dictionary<string, T> Extensions { get; private set; }
        public string FriendlyName { get; private set; }
        public string Name { get; private set; }
        public ExtensionLibraryType LibraryType { get; private set; }
        public Guid Guid { get; private set; }
        public Version Version { get; private set; }
        protected ExtensionLibraryBase(string friendlyName, string name, ExtensionLibraryType type, Guid g, RequiresDependencyAttribute[] deps, Version v)
        {
            FriendlyName = friendlyName;
            Name = name;
            LibraryType = type;
            Guid = g;
            Version = v;
            Extensions = new Dictionary<string, T>();
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