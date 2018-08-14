using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Core.IOC;

namespace RSPM
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection UsePackageManager<TPackageManagerImpl>(this IServiceCollection services) where TPackageManagerImpl : IPackageManager
        {
            return services.AddTransient<IPackageManager, TPackageManagerImpl>();
        }
    }
}