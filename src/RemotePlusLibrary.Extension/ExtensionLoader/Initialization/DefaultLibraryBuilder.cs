using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.Extension.CommandSystem;

namespace RemotePlusLibrary.Extension.ExtensionLoader.Initialization
{
    /// <summary>
    /// The default implementation of the library builder.
    /// </summary>
    public class DefaultLibraryBuilder : ILibraryBuilder
    {
        public DefaultLibraryBuilder(string friendlyName, string name, string version, NetworkSide type)
        {
            FriendlyName = friendlyName;
            Name = name;
            Version = version;
            LibraryType = type;
        }
        public string FriendlyName { get; private set; }

        public string Name { get; private set; }

        public string Version { get; private set; }

        public NetworkSide LibraryType { get; private set; }

        public void AddCommandClass<TCommandClass>() where TCommandClass : ICommandClass
        {
            IOCContainer.Provider.Bind<ICommandClass>().To(typeof(TCommandClass)).InSingletonScope();
        }

        public void AddCommandClass(ICommandClass commandClass)
        {
            IOCContainer.Provider.Bind<ICommandClass>().ToConstant(commandClass);
        }

        void ILibraryBuilder.SubscribeToEventBus<TMessage>(Action<TMessage> subscriber)
        {
            GlobalServices.EventBus.Subscribe(subscriber);
        }

        void ILibraryBuilder.SubscribeToEventBus<TMessage>(Action<TMessage> subscriber, Func<TMessage, bool> condition)
        {
            GlobalServices.EventBus.Subscribe(subscriber, condition);
        }
    }
}
