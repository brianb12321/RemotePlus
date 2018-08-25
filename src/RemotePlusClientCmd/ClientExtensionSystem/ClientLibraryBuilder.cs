using RemotePlusLibrary.Extension.ExtensionLoader;
using RemotePlusLibrary.Extension.ExtensionLoader.Initialization;
using System;
using TinyMessenger;

namespace RemotePlusClientCmd.ClientExtensionSystem
{
    internal class ClientLibraryBuilder : ILibraryBuilder
    {
        public string Name { get; set; }
        public string FriendlyName { get; set; }
        public string Version { get; set; }
        public ExtensionLibraryType LibraryType { get; set; }

        public ClientLibraryBuilder(string name, string friendlyName, string version, ExtensionLibraryType libraryType)
        {
            this.Name = name;
            this.FriendlyName = friendlyName;
            this.Version = version;
            this.LibraryType = libraryType;
        }

        public void SubscribeToEventBus<TMessage>(Action<TMessage> subscriber) where TMessage : class, ITinyMessage
        {
            ClientCmdManager.EventBus.Subscribe(subscriber);
        }

        public void SubscribeToEventBus<TMessage>(Action<TMessage> subscriber, Func<TMessage, bool> condition) where TMessage : class, ITinyMessage
        {
            ClientCmdManager.EventBus.Subscribe(subscriber, condition);
        }
    }
}