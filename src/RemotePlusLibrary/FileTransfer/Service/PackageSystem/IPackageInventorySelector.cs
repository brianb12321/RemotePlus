using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.FileTransfer.Service.PackageSystem
{
    public interface IPackageInventorySelector
    {
        IPackageInventory<TPackage> GetInventory<TPackage>(string name) where TPackage : Package;
        void Route(Package package);
        void AddRoute(Action<Package> router);
        void AddPackageInventory<TPackage, TInventoryImpl>(string name) where TPackage : Package
            where TInventoryImpl : IPackageInventory<TPackage>;
    }
}