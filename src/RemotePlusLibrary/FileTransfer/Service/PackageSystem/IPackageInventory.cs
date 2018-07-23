using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.FileTransfer.Service.PackageSystem
{
    /// <summary>
    /// Handles receiving and dispatching packages to registered package listeners
    /// </summary>
    public interface IPackageInventory<TPackage> where TPackage : Package
    {
        /// <summary>
        /// Registers a listener that will be called when a package comes into the inventory.
        /// </summary>
        /// <param name="listener">The function that will handle the package.</param>
        PackageListener<TPackage> Receive(Action<TPackage> listener, bool keepActive = false);
        void UnRegisterListener(PackageListener<TPackage> listener);
        void Dispatch(TPackage p);
    }
}