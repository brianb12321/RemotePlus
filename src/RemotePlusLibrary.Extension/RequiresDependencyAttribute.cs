using RemotePlusLibrary.Extension.ExtensionLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension
{
    /// <summary>
    /// Specifies that the current extension library requires another file.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class RequiresDependencyAttribute : Attribute
    {
        /// <summary>
        /// The name of the file that the extension libary is dependent on.
        /// </summary>
        public string DependencyName { get; private set; }
        /// <summary>
        /// The version of the dependent library.
        /// </summary>
        public Version Version { get; private set; }
        public RequiresDependencyAttribute(string name, string version)
        {
            DependencyName = name;
            Version = Version.Parse(version);
            LoadIfNotLoaded = false;
            DependencyType = DependencyType.RemotePlusLib;
        }
        /// <summary>
        /// The type of the dependency.
        /// </summary>
        public DependencyType DependencyType { get; set; }
        /// <summary>
        /// If the dependency is a extension library, you can load the extension library automatically.
        /// </summary>
        public bool LoadIfNotLoaded { get; set; }
    }
}
