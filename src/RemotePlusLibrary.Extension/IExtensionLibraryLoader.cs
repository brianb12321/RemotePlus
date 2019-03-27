using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension
{
    public interface IExtensionLibraryLoader
    {
        event EventHandler<ExtensionLibrary> LibraryAdded;
        int Count { get; }
        ExtensionLibrary LoadFromAssembly(Assembly assembly);
        ExtensionLibrary[] LoadFromFolder(string folder);
        TModuleType[] GetAllModules<TModuleType>() where TModuleType : IExtensionModule;
        ExtensionLibrary[] GetAllLibraries();
        void RunPostInit();
    }
}