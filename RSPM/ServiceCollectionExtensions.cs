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
            services.AddTransient<IPackageDownloader, DefaultPackageDownloader>();
            return services.AddTransient<IPackageManager, TPackageManagerImpl>();
        }
        public static IServiceCollection UsePackageManager<TPackageManagerImpl>(this IServiceCollection services, Action<PackageManagerBuilder> builder) where TPackageManagerImpl : IPackageManager
        {
            PackageManagerBuilder b = new PackageManagerBuilder();
            builder?.Invoke(b);
            return services.AddTransient<IPackageManager, TPackageManagerImpl>();
        }
    }
}