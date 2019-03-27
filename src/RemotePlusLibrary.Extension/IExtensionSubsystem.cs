using RemotePlusLibrary.Core;
using RemotePlusLibrary.Core.EventSystem;
using RemotePlusLibrary.Core.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension
{
    /// <summary>
    /// Represents a logical subsystem in an application. Each subsystem manages different types of <see cref="IExtensionModule"/> in the system.
    /// All subsystems will use the <see cref="IExtensionLibraryLoader"/> to retrieve specific modules.
    /// </summary>
    public interface IExtensionSubsystem<out TModuleType> where TModuleType : IExtensionModule
    {
        /// <summary>
        /// The current environment of the host.
        /// </summary>
        IEnvironment Side { get; }
        /// <summary>
        /// Gets a service from the host's IOC container.
        /// </summary>
        /// <typeparam name="TService">The type of service to get.</typeparam>
        /// <returns></returns>
        TService GetService<TService>();
        /// <summary>
        /// Special event bus for inter-extension library communication.
        /// </summary>
        IEventBus ModuleEventBus { get; }
        TModuleType[] GetAllModules();
        void Init();
    }
}