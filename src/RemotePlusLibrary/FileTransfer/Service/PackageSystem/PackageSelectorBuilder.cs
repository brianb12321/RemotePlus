using RemotePlusLibrary.Core.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;

namespace RemotePlusLibrary.FileTransfer.Service.PackageSystem
{
    /// <summary>
    /// Helps inject package inventories services.
    /// </summary>
    public class PackageSelectorBuilder
    {
        private IKernel currentPackageInventorySelectorProvider;
        public PackageSelectorBuilder(IKernel provider)
        {
            currentPackageInventorySelectorProvider = provider;
        }
        public PackageSelectorBuilder AddPackageInventory<TPackage, TPackageInventory>(string inventoryName) where TPackage : Package
            where TPackageInventory : IPackageInventory<TPackage>
        {
            currentPackageInventorySelectorProvider.Bind<IPackageInventory<TPackage>>().To(typeof(TPackageInventory)).InSingletonScope().Named(inventoryName);
            return this;
        }
    }
}