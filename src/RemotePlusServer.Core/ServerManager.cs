using Logging;
using Ninject;
using RemotePlusLibrary;
using RemotePlusLibrary.Configuration.ServerSettings;
using RemotePlusLibrary.Scripting;
using RemotePlusServer.Core.ExtensionSystem;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusServer.Core
{
    public static class ServerManager
    {
        public static IRemotePlusService<ServerRemoteInterface> ServerRemoteService => IOCContainer.Kernel.Get<IRemotePlusService<ServerRemoteInterface>>();
        /// <summary>
        /// The global logger for the server.
        /// TODO: Create ILogger and add it to the IOC container.
        /// </summary>
        public static CMDLogging Logger { get; } = new CMDLogging();
        /// <summary>
        /// The main server configuration. Provides settings for the main server.
        /// </summary>
        public static ServerSettings DefaultSettings { get; set; }
        /// <summary>
        /// The global container that all house the libraries that are loaded into the system.
        /// </summary>
        public static ServerExtensionLibraryCollection DefaultCollection { get; } = new ServerExtensionLibraryCollection();
        public static Guid ServerGuid { get; set; }
        public static ScriptBuilder ScriptBuilder { get; } = new ScriptBuilder();
        public static IRemotePlusService<FileTransferServciceInterface> FileTransferService => IOCContainer.Kernel.Get<IRemotePlusService<FileTransferServciceInterface>>();
    }
}