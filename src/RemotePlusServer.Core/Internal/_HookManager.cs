using RemotePlusLibrary.Extension.HookSystem;
using RemotePlusLibrary.Extension.ExtensionLoader.Initialization;
using RemotePlusServer.Core;
using RemotePlusServer.Core.ExtensionSystem;
using BetterLogger;
using RemotePlusLibrary;

namespace RemotePlusServer.Internal
{
    /// <summary>
    /// Provides functionality that is used to locate and execute server hooks. THIS IS A INTERNAL CLASS
    /// </summary>
    public static class _HookManager
    {
        public static void RunHooks(string hookCategory, HookArguments args)
        {
            GlobalServices.Logger.Log($"Calling hooks. Event: {hookCategory}", LogLevel.Info);
            foreach(ServerExtensionLibrary lib in ServerManager.DefaultCollection.Libraries.Values)
            {
                if (lib.Hooks.ContainsKey(hookCategory))
                {
                    foreach(ServerHook hook in lib.Hooks[hookCategory])
                    {
                        GlobalServices.Logger.Log($"Calling hook. Library: {lib.FriendlyName}", LogLevel.Info);
                        hook(args);
                    }
                }
            }
        }
    }
}