﻿using BetterLogger;
using RemotePlusLibrary;
using RemotePlusLibrary.Configuration.ServerSettings;
using RemotePlusLibrary.Scripting;
using RemotePlusServer.Core.ExtensionSystem;
using System;
using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.Security.AccountSystem;
using RemotePlusLibrary.Core.EventSystem;
using RemotePlusLibrary.Configuration;
using RemotePlusLibrary.ServiceArchitecture;
using RemotePlusLibrary.Extension.ExtensionLoader;

namespace RemotePlusServer.Core
{
    public static class ServerManager
    {
        public static IServiceManager DefaultServiceManager => IOCContainer.GetService<IServiceManager>();
        public static IRemotePlusService<ServerRemoteInterface> ServerRemoteService => DefaultServiceManager.GetService<ServerRemoteInterface>();
        /// <summary>
        /// The main server configuration. Provides settings for the main server.
        /// </summary>
        public static ServerSettings DefaultSettings { get; set; } = new ServerSettings();
        /// <summary>
        /// The global container that all house the libraries that are loaded into the system.
        /// </summary>
        public static ExtensionLibraryCollectionBase<ServerExtensionLibrary> DefaultCollection => IOCContainer.GetService<ExtensionLibraryCollectionBase<ServerExtensionLibrary>>();
        public static IAccountManager AccountManager => IOCContainer.GetService<IAccountManager>();
        public static Guid ServerGuid { get; set; }
        public static IScriptingEngine ScriptBuilder => IOCContainer.GetService<IScriptingEngine>();
        public static bool IsService { get; set; }
        public static IRemotePlusService<FileTransferServciceInterface> FileTransferService => DefaultServiceManager.GetService<FileTransferServciceInterface>();
        public static IEventBus EventBus => IOCContainer.GetService<IEventBus>();
        public static IConfigurationDataAccess DataAccess => IOCContainer.GetService<IConfigurationDataAccess>("DefaultConfigDataAccess");
    }
}