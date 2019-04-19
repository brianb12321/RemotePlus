using RemotePlusLibrary.Core;
using RemotePlusLibrary.Core.IOC;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace RemotePlusLibrary.Extension
{
    public class ExtensionLibrary
    {
        public string Name { get; private set; }
        public string FriendlyName { get; private set; }
        public NetworkSide LibraryType { get; private set; }
        public Version Version { get; private set; }
        public Guid UniqueID { get; private set; }
        public IExtensionModule[] Modules { get; private set; }
        private ILibraryStartup _startup;
        private ExtensionLibrary(string name, string friendlyName, Version version, Guid id, NetworkSide type, IExtensionModule[] modules, ILibraryStartup startup)
        {
            Name = name;
            FriendlyName = friendlyName;
            Version = version;
            UniqueID = id;
            LibraryType = type;
            Modules = modules;
            _startup = startup;
        }
        public void RunPostInit()
        {
            _startup?.PostInit();
        }
        public static ExtensionLibrary LoadFromAssembly(Assembly a)
        {
            ExtensionLibraryAttribute attrib = a.GetCustomAttribute<ExtensionLibraryAttribute>();
            if(attrib != null)
            {
                IEnvironment env = IOCContainer.GetService<IEnvironment>();
                if(!attrib.LibraryType.HasFlag(env.ExecutingSide))
                {
                    throw new InvalidExtensionLibraryException($"Assembly: \"{a.GetName().FullName}\" could not be loaded: Extension \"{attrib.FriendlyName}\" requires a network side of {attrib.LibraryType.ToString()}");
                }
                ILibraryStartup startup = null;
                if (attrib.Startup != null)
                {
                    startup = (ILibraryStartup)Activator.CreateInstance(attrib.Startup);
                    startup.Init(IOCContainer.Provider);
                }
                List<IExtensionModule> _modules = new List<IExtensionModule>();
                foreach(IExtensionModule module in a.GetTypes().Where(
                    t => t.IsClass && !t.IsAbstract && typeof(IExtensionModule).IsAssignableFrom(t) && t.GetCustomAttribute<ExtensionModuleAttribute>() != null)
                    .Select(t => (IExtensionModule)Activator.CreateInstance(t)))
                {
                    _modules.Add(module);
                }
                return new ExtensionLibrary(attrib.Name, attrib.FriendlyName, a.GetName().Version, Guid.TryParse(attrib.Guid, out Guid value) ? value : Guid.NewGuid(), attrib.LibraryType, _modules.ToArray(), startup);
            }
            else
            {
                throw new InvalidExtensionLibraryException($"Assembly: \"{a.GetName().FullName}\" could not be loaded: ExtensionLibraryAttribute missing.");
            }
        }
    }
}