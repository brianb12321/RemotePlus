using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.ExtensionLoader
{
    /// <summary>
    /// Provides a proxy for accessing an extension library from a different AppDomain
    /// </summary>
    public class ExtensionLibraryLoader : MarshalByRefObject
    {
        public Assembly GetOrLoadAssembly(string assemblyName)
        {
            return Assembly.LoadFile(assemblyName);
        }
        public Assembly GetOrLoadAssembly(byte[] assembly)
        {
            return Assembly.Load(assembly);
        }
    }
}