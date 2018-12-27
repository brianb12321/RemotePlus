using BetterLogger;
using RemotePlusLibrary.Extension.ExtensionLoader.Initialization;
using System;
using System.Collections.Generic;

namespace RemotePlusLibrary.Extension.ExtensionLoader
{
    public abstract class ExtensionLibraryCollectionBase<T> where T : ExtensionLibraryBase
    {
        public Dictionary<string, T> Libraries { get; private set; }
        public ExtensionLibraryCollectionBase()
        {
            Libraries = new Dictionary<string, T>();
        }
        public abstract void LoadExtension(string path, IInitEnvironment env);
        public abstract void LoadExtension(byte[] data, IInitEnvironment env);
        public void RunPostInit()
        {
            foreach(ExtensionLibraryBase lib in Libraries.Values)
            {
                lib.Startup.PostInit();
            }
        }
    }
}
