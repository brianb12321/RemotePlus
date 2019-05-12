using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Core.EventSystem;
using RemotePlusLibrary.Core.IOC;

namespace RemotePlusLibrary.Extension
{
    public abstract class BaseExtensionSubsystem<TSystem, TModule> : IExtensionSubsystem<TModule>
        where TSystem : IExtensionSubsystem<TModule>
        where TModule : IExtensionModule
    {
        protected IExtensionLibraryLoader ExtensionLoader { get; private set; }
        public IApplication Side => GlobalServices.RunningApplication;
        public IEventBus ModuleEventBus { get; private set; } = new EventBus(GlobalServices.Logger);
        protected BaseExtensionSubsystem(IExtensionLibraryLoader loader)
        {
            ExtensionLoader = loader;
        }
        public TModule[] GetAllModules()
        {
            return ExtensionLoader.GetAllModules<TModule>();
        }

        public TService GetService<TService>()
        {
            return IOCContainer.GetService<TService>();
        }

        public virtual void Init()
        {
            ExtensionLoader.GetAllModules<TModule>().ToList().ForEach(m => m.InitializeServices(IOCContainer.Provider));
        }
    }
}