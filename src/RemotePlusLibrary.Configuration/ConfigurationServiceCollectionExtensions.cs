using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BetterLogger;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Core.IOC;

namespace RemotePlusLibrary.Configuration
{
    public static class ConfigurationServiceCollectionExtensions
    {
        public static IServiceCollection UseConfigurationDataAccess<TDataAccessImpl>(this IServiceCollection services)
        {
            return services.AddSingletonNamed<IConfigurationDataAccess, TDataAccessImpl>("DefaultDataAccess");
        }
    }
}