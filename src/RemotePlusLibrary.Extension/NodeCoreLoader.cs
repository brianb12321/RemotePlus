using RemotePlusLibrary.Core.IOC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Core.NodeStartup;

namespace RemotePlusLibrary.Extension
{
    public static class NodeCoreLoader
    {
        public static INodeCoreStartup<TNodeBuilder> LoadNodeCore<TNodeBuilder>(Assembly a, NetworkSide side) where TNodeBuilder : INodeBuilder<TNodeBuilder>
        {
            var attribute = a.GetCustomAttribute<NodeCoreExtensionLibraryAttribute>();
            if(attribute != null)
            {
                if (attribute.LibraryType.HasFlag(side))
                {
                    var startupType = a.GetTypes()
                        .FirstOrDefault(t => typeof(INodeCoreStartup<TNodeBuilder>).IsAssignableFrom(t));
                    if (startupType != null)
                    {
                        return (INodeCoreStartup<TNodeBuilder>)Activator.CreateInstance(startupType);
                    }
                    else return null;
                }
                else return null;
            }
            return null;
        }

        public static INodeCoreStartup<TBuilder> LoadByScan<TBuilder>(string resourceName, NetworkSide side)
            where TBuilder : INodeBuilder<TBuilder>
        {
            bool foundCore = false;
            foreach (string coreFile in Directory.GetFiles(Environment.CurrentDirectory))
            {
                if (Path.GetExtension(coreFile) == ".dll")
                {
                    var core = LoadNodeCore<TBuilder>(Assembly.LoadFile(coreFile), side);
                    if (core != null)
                    {
                        foundCore = true;
                        return core;
                    }
                }
            }
            if (foundCore == false)
            {
                var embeddedCore = searchEmbeddedServerCore<TBuilder>(resourceName, side);
                if (embeddedCore != null)
                {
                    return embeddedCore;
                }
                else
                {
                    throw new Exception("FATAL ERROR: A server core is not present. Cannot start server.");
                }
            }
            return null;
        }

        private static INodeCoreStartup<TBuilder> searchEmbeddedServerCore<TBuilder>(string resourceName, NetworkSide side) where TBuilder : INodeBuilder<TBuilder>
        {
            try
            {
                Console.WriteLine("Attempting to load server core from embedded resource.");
                var stream = new BinaryReader(Assembly.GetEntryAssembly().GetManifestResourceStream(resourceName));
                byte[] buffer = new byte[stream.BaseStream.Length];
                stream.Read(buffer, 0, buffer.Length);
                return LoadNodeCore<TBuilder>(Assembly.Load(buffer), side);
            }
            catch (Exception ex)
            {
                throw new AggregateException(ex, new Exception($"ERROR: Unable to load resource: {ex.Message}"));
            }
        }
    }
}