using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.FileTransfer.Service.PackageSystem
{
    public interface IPackageInventorySelector
    {
        /// <summary>
        /// The container that houses all the necessary objects that each package inventory needs.
        /// </summary>
        IKernel PackageInventoryProvider { get; }
        /// <summary>
        /// Retrieves a package inventory from the <see cref="PackageInventoryProvider"/>
        /// </summary>
        /// <typeparam name="TPackage">The type of package inventory to retrieve. A package inventory type is the type of package that the inventory houses.</typeparam>
        /// <param name="name">A unique identifying name for the package inventory.</param>
        /// <returns>The package inventory to be requested.</returns>
        IPackageInventory<TPackage> GetInventory<TPackage>(string name) where TPackage : Package;
        /// <summary>
        /// Sends a package through the package inventory selector. A package goes through a series of routers."/>
        /// </summary>
        /// <param name="package">The package to be sent.</param>
        void Route(Package package);
        /// <summary>
        /// Adds a route to the package inventory selector.
        /// </summary>
        /// <param name="router">The route to be added/</param>
        void AddRoute(Action<Package> router);
        /// <summary>
        /// Adds a package inventory to the <see cref="PackageInventoryProvider"/>, a collection of objects used by the package inventory selector.
        /// </summary>
        /// <typeparam name="TPackage">The package inventory type.</typeparam>
        /// <typeparam name="TInventoryImpl">The implementation that implements the <see cref="IPackageInventory{TPackage}"/> interface. The implementation must match the package type of TPackage/></typeparam>
        /// <param name="name">A unique that name that identifies the package inventory in the system.</param>
        void AddPackageInventory<TPackage, TInventoryImpl>(string name) where TPackage : Package
            where TInventoryImpl : IPackageInventory<TPackage>;
    }
}