using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.FileTransfer.Service.PackageSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusServer.Core
{
    public static class ServerOnlyServiceCollectionExtensions
    {
        public static IServiceCollection UsePackageInventorySelector<TPackageInventorySelector>(this IServiceCollection services, Action<PackageSelectorBuilder> builder)
            where TPackageInventorySelector : IPackageInventorySelector
        {
            var output = services.AddSingleton<IPackageInventorySelector, TPackageInventorySelector>();
            builder?.Invoke(new PackageSelectorBuilder());
            return output;
        }
    }
}
