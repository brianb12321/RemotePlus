using RemotePlusLibrary.Extension.HookSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Extension;
using RemotePlusServer.ExtensionSystem;
using System.ServiceModel;
using RemotePlusLibrary.Extension.ExtensionLoader.Initialization;

namespace RemotePlusServer.Internal
{
    /// <summary>
    /// Provides functionality that is used to locate and execute server hooks. THIS IS A INTERNAL CLASS
    /// </summary>
    internal static class _HookManager
    {
        public static void RunHooks(string hookCategory, HookArguments args)
        {
            ServerManager.Logger.AddOutput($"Calling hooks. Event: {hookCategory}", Logging.OutputLevel.Info);
            foreach(ServerExtensionLibrary lib in ServerManager.DefaultCollection.Libraries.Values)
            {
                if (lib.Hooks.ContainsKey(hookCategory))
                {
                    foreach(ServerHook hook in lib.Hooks[hookCategory])
                    {
                        ServerManager.Logger.AddOutput($"Calling hook. Library: {lib.FriendlyName}", Logging.OutputLevel.Info);
                        hook(args);
                    }
                }
            }
        }
    }
}