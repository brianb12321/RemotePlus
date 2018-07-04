using BetterLogger;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.IOC
{
    public static class IOCContainer
    {
        public static IKernel Provider { get; set; } = new StandardKernel();
    }
    public static class GlobalServices
    {
        /// <summary>
        /// The global logger for the server.
        /// </summary>
        public static ILogFactory Logger => IOCContainer.Provider.Get<ILogFactory>();
    }
}
