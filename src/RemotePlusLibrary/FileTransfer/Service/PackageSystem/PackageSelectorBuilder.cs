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
        public void AddPackageInventory<TPackage, TPackageInventory>(string inventoryName) where TPackage : Package
            where TPackageInventory : IPackageInventory<TPackage>
        {
            IOCContainer.Provider.Bind<IPackageInventory<TPackage>>().To(typeof(TPackageInventory)).InSingletonScope().Named(inventoryName);
        }
    }
}
