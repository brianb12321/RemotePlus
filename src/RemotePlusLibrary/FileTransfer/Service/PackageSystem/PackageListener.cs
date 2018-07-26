using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.FileTransfer.Service.PackageSystem
{
    public class PackageListener<TPackage> where TPackage : Package
    {
        /// <summary>
        /// The method that should be executed when the specified package has arrived.
        /// </summary>
        public Action<TPackage> ListenerDelegate { get; private set; }
        /// <summary>
        /// Determines whether to not dispose the package after the listener has finished.
        /// </summary>
        public bool KeepAlive { get; set; }
        public PackageListener(Action<TPackage> ld)
        {
            ListenerDelegate = ld;
        }
    }
}