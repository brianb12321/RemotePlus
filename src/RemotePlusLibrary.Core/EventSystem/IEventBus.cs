using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyMessenger;

namespace RemotePlusLibrary.Core.EventSystem
{
    /// <summary>
    /// An event bus holds subscribers that can be called when a particular event gets raised.
    /// </summary>
    public interface IEventBus
    {
        TinyMessageSubscriptionToken Subscribe<TMessage>(Action<TMessage> subscriber) where TMessage : class, ITinyMessage;
        TinyMessageSubscriptionToken Subscribe<TMessage>(Action<TMessage> subscriber, Func<TMessage, bool> condition) where TMessage : class, ITinyMessage;
        void Publish<TMessage>(TMessage message) where TMessage : class, ITinyMessage;
        void Publish(ITinyMessage message);
        void UnSubscribe<TMessage>(TinyMessageSubscriptionToken token) where TMessage : class, ITinyMessage;
        void RemoveEventProxy();
    }
}