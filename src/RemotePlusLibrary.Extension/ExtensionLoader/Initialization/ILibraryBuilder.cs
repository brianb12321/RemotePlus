using RemotePlusLibrary.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyMessenger;

namespace RemotePlusLibrary.Extension.ExtensionLoader.Initialization
{
    public interface ILibraryBuilder
    {
        string FriendlyName { get; }
        string Name { get; }
        string Version { get; }
        NetworkSide LibraryType { get; }
        void SubscribeToEventBus<TMessage>(Action<TMessage> subscriber) where TMessage : class, ITinyMessage;
        void SubscribeToEventBus<TMessage>(Action<TMessage> subscriber, Func<TMessage, bool> condition) where TMessage : class, ITinyMessage;
    }
}