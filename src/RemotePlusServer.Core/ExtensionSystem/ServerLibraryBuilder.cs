using RemotePlusLibrary.Core;
using RemotePlusLibrary.Extension.ExtensionLoader;
using RemotePlusLibrary.Extension.ExtensionLoader.Initialization;
using System;
using TinyMessenger;

namespace RemotePlusServer.Core.ExtensionSystem
{
    public class ServerLibraryBuilder : ILibraryBuilder
    {
        public const string LOGIN_HOOK = "Login";
        public const string RUN_COMMAND_HOOK = "RunCommand";
        public ServerLibraryBuilder(string n, string fn, string v, NetworkSide lt)
        {
            Name = n;
            FriendlyName = fn;
            Version = v;
            LibraryType = lt;
        }

        public string Name { get; private set; }

        public string Version { get; private set; }

        public NetworkSide LibraryType { get; private set; }

        public string FriendlyName { get; private set; }

        public void SubscribeToEventBus<TMessage>(Action<TMessage> subscriber) where TMessage : class, ITinyMessage
        {
            ServerManager.EventBus.Subscribe(subscriber);
        }

        public void SubscribeToEventBus<TMessage>(Action<TMessage> subscriber, Func<TMessage, bool> condition) where TMessage : class, ITinyMessage
        {
            ServerManager.EventBus.Subscribe(subscriber, condition);
        }
    }
}