using BetterLogger;
using RemotePlusLibrary.Core.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;

namespace RemotePlusLibrary
{
    public static class GlobalServices
    {
        /// <summary>
        /// The global logger for the server.
        /// </summary>
        public static ILogFactory Logger => IOCContainer.GetService<ILogFactory>();
        public static Configuration.IConfigurationDataAccess DataAccess => IOCContainer.GetService<Configuration.IConfigurationDataAccess>("DefaultConfigDataAccess");
    }
}
