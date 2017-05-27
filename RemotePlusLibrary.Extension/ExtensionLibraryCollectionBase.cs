using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension
{
    public abstract class ExtensionLibraryCollectionBase<T, Ed>
    {
        public Dictionary<string, T> Libraries { get; private set; }
        public ExtensionLibraryCollectionBase()
        {
            Libraries = new Dictionary<string, T>();
        }
        public abstract Dictionary<string, Ed> GetAllExtensions();
    }
}
