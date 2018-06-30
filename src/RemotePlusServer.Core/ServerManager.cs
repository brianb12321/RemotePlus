using BetterLogger;
using RemotePlusLibrary;
using RemotePlusLibrary.Configuration.ServerSettings;
using RemotePlusLibrary.Scripting;
using RemotePlusServer.Core.ExtensionSystem;
using System;
using Ninject;

namespace RemotePlusServer.Core
{
    public static class ServerManager
    {
        public static IRemotePlusService<ServerRemoteInterface> ServerRemoteService => (IRemotePlusService<ServerRemoteInterface>)IOCContainer.Provider.GetService(typeof(IRemotePlusService<ServerRemoteInterface>));
        /// <summary>
        /// The global logger for the server.
        /// TODO: Create ILogger and add it to the IOC container.
        /// </summary>
        public static ILogFactory Logger => IOCContainer.Provider.Get<ILogFactory>();
        /// <summary>
        /// The main server configuration. Provides settings for the main server.
        /// </summary>
        public static ServerSettings DefaultSettings { get; set; } = new ServerSettings();
        /// <summary>
        /// The global container that all house the libraries that are loaded into the system.
        /// </summary>
        public static ServerExtensionLibraryCollection DefaultCollection { get; } = new ServerExtensionLibraryCollection();
        public static Guid ServerGuid { get; set; }
        public static ScriptBuilder ScriptBuilder { get; } = new ScriptBuilder();
        public static IRemotePlusService<FileTransferServciceInterface> FileTransferService => (IRemotePlusService<FileTransferServciceInterface>)IOCContainer.Provider.GetService(typeof(IRemotePlusService<FileTransferServciceInterface>));
    }
}