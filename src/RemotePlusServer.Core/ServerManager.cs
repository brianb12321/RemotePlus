using BetterLogger;
using RemotePlusLibrary;
using RemotePlusLibrary.Configuration.ServerSettings;
using RemotePlusLibrary.Scripting;
using RemotePlusServer.Core.ExtensionSystem;
using System;
using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.FileTransfer.Service.PackageSystem;
using RemotePlusLibrary.Security.AccountSystem;
using RemotePlusLibrary.Extension.EventSystem;

namespace RemotePlusServer.Core
{
    public static class ServerManager
    {
        public static IRemotePlusService<ServerRemoteInterface> ServerRemoteService => IOCContainer.GetService<IRemotePlusService<ServerRemoteInterface>>();
        /// <summary>
        /// The main server configuration. Provides settings for the main server.
        /// </summary>
        public static ServerSettings DefaultSettings { get; set; } = new ServerSettings();
        /// <summary>
        /// The global container that all house the libraries that are loaded into the system.
        /// </summary>
        public static ServerExtensionLibraryCollection DefaultCollection { get; } = new ServerExtensionLibraryCollection();
        public static IAccountManager AccountManager => IOCContainer.GetService<IAccountManager>();
        public static Guid ServerGuid { get; set; }
        public static ScriptBuilder ScriptBuilder => IOCContainer.GetService<ScriptBuilder>();
        public static IPackageInventorySelector DefaultPackageInventorySelector => IOCContainer.GetService<IPackageInventorySelector>();
        public static bool IsService { get; set; }
        public static IRemotePlusService<FileTransferServciceInterface> FileTransferService => IOCContainer.GetService<IRemotePlusService<FileTransferServciceInterface>>();
        public static IEventBus EventBus => IOCContainer.GetService<IEventBus>();
    }
}