using BetterLogger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension
{
    public class DefaultExtensionLoader : IExtensionLibraryLoader
    {
        List<ExtensionLibrary> _libs = new List<ExtensionLibrary>();
        ILogFactory _logger;
        public DefaultExtensionLoader(ILogFactory logger)
        {
            _logger = logger;
        }

        public int Count => _libs.Count;

        public event EventHandler<ExtensionLibrary> LibraryAdded;

        public ExtensionLibrary[] GetAllLibraries()
        {
            return _libs.ToArray();
        }

        public TModuleType[] GetAllModules<TModuleType>() where TModuleType : IExtensionModule
        {
            List<TModuleType> result = new List<TModuleType>();
            foreach(ExtensionLibrary libs in _libs)
            {
                foreach(IExtensionModule module in libs.Modules)
                {
                    if(module is TModuleType)
                    {
                        result.Add((TModuleType)module);
                    }
                }
            }
            return result.ToArray();
        }

        public ExtensionLibrary LoadFromAssembly(Assembly assembly)
        {
            try
            {
                ExtensionLibrary loadedLib = ExtensionLibrary.LoadFromAssembly(assembly);
                LibraryAdded?.Invoke(this, loadedLib);
                _libs.Add(loadedLib);
                return loadedLib;
            }
            catch (InvalidExtensionLibraryException ex)
            {
                _logger.Log($"An error occured loading extension library \"{assembly.GetName().FullName}\": {ex.Message}", LogLevel.Warning);
                return null;
            }
        }

        public ExtensionLibrary[] LoadFromFolder(string folder)
        {
            List<ExtensionLibrary> _libs = new List<ExtensionLibrary>();
            if(Directory.Exists(folder))
            {
                foreach(string file in Directory.GetFiles(folder, "*.dll", SearchOption.AllDirectories))
                {
                    _logger.Log($"Found extension library \"{Path.GetFileName(file)}\"", LogLevel.Info, "Extension Loader");
                    _libs.Add(LoadFromAssembly(Assembly.LoadFrom(file)));
                }
                return _libs.ToArray();
            }
            else
            {
                _logger.Log($"{folder} does not exist.", LogLevel.Info, "Extension Loader");
                return null;
            }
        }

        public void RunPostInit()
        {
            _libs.ForEach(l => l.RunPostInit());
        }
    }
}