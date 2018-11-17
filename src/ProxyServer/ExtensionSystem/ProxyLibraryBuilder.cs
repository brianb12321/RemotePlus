using System;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Extension.ExtensionLoader;
using RemotePlusLibrary.Extension.ExtensionLoader.Initialization;

namespace ProxyServer.ExtensionSystem
{
    internal class ProxyLibraryBuilder : ILibraryBuilder
    {
        public string Name { get; private set; }
        public string FriendlyName {get; private set;}
        public string Version { get; private set; }
        public NetworkSide LibraryType { get; private set; }

        public ProxyLibraryBuilder(string name, string friendlyName, string version, NetworkSide libraryType)
        {
            this.Name = name;
            this.FriendlyName = friendlyName;
            this.Version = version;
            this.LibraryType = libraryType;
        }

        void ILibraryBuilder.SubscribeToEventBus<TMessage>(Action<TMessage> subscriber)
        {
            throw new NotImplementedException();
        }

        void ILibraryBuilder.SubscribeToEventBus<TMessage>(Action<TMessage> subscriber, Func<TMessage, bool> condition)
        {
            throw new NotImplementedException();
        }
    }
}