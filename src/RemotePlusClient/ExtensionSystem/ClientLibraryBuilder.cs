using RemotePlusLibrary.Core;
using RemotePlusLibrary.Extension.ExtensionLoader;
using RemotePlusLibrary.Extension.ExtensionLoader.Initialization;
using System;
using System.Collections.Generic;
using TinyMessenger;

namespace RemotePlusClient.ExtensionSystem
{
    internal class ClientLibraryBuilder : ILibraryBuilder
    {

        public ClientLibraryBuilder(string name, string friendlyName, string version, NetworkSide libraryType)
        {
            Name = name;
            FriendlyName = friendlyName;
            Version = version;
            LibraryType = libraryType;
        }

        public string FriendlyName { get; private set; }

        public string Name { get; private set; }

        public string Version { get; private set; }

        public NetworkSide LibraryType { get; private set; }

        public void SubscribeToEventBus<TMessage>(Action<TMessage> subscriber) where TMessage : class, ITinyMessage
        {
            ClientApp.EventBus.Subscribe(subscriber);   
        }

        public void SubscribeToEventBus<TMessage>(Action<TMessage> subscriber, Func<TMessage, bool> condition) where TMessage : class, ITinyMessage
        {
            ClientApp.EventBus.Subscribe(subscriber, condition);
        }
    }
}