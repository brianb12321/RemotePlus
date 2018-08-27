using BetterLogger;
using RemotePlusLibrary.Extension.ExtensionLoader.Initialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.ExtensionLoader
{
    public abstract class ExtensionLibraryCollectionBase<T, E> where T : ExtensionLibraryBase<E> 
    {
        public Dictionary<string, T> Libraries { get; private set; }
        public ExtensionLibraryCollectionBase()
        {
            Libraries = new Dictionary<string, T>();
        }
        public abstract Dictionary<string, E> GetAllExtensions();
        public abstract void LoadExtension(string path, Action<string, LogLevel> callback, IInitEnvironment env);
        public abstract void LoadExtension(byte[] data, Action<string, LogLevel> callback, IInitEnvironment env);
    }
}
