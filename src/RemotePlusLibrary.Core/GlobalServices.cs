using BetterLogger;
using RemotePlusLibrary.Core.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using RemotePlusLibrary.Core.EventSystem;

namespace RemotePlusLibrary.Core
{
    /// <summary>
    /// Provides an easy access to services used by all RemotePlus programs and extensions.
    /// </summary>
    public static class GlobalServices
    {
        /// <summary>
        /// The global logger for the server.
        /// </summary>
        public static ILogFactory Logger => IOCContainer.GetService<ILogFactory>();
        public static IEventBus EventBus => IOCContainer.GetService<IEventBus>();
    }
}
