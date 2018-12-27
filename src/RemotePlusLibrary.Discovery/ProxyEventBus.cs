using BetterLogger;
using RemotePlusLibrary.Core.EventSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyMessenger;

namespace RemotePlusLibrary.Discovery
{
    public class ProxyEventBus : IEventBus
    {
        private ILogFactory _factory;
        private IRemoteWithProxy _proxy;
        private IEventBus _bus;
        public ProxyEventBus(IEventBus bus, ILogFactory factory, IRemoteWithProxy proxy)
        {
            _factory = factory;
            _proxy = proxy;
            _bus = bus;
            LoggerEventProxy.Instance.DeliveringEvent += Instance_DeliveringEvent;
        }

        private void Instance_DeliveringEvent(object sender, EventDeliveryEventArgs e)
        {
            _factory.Log($"Event raised '{e.EventType}'", LogLevel.Info, "EventBus");
        }

        public TinyMessageSubscriptionToken Subscribe<TMessage>(Action<TMessage> subscriber) where TMessage : class, ITinyMessage
        {
            var token = _bus.Subscribe(subscriber);
            _factory.Log($"Subscriber of type '{typeof(TMessage).Name}' added to event bus.", LogLevel.Info, "EventBus");
            return token;
        }
        public TinyMessageSubscriptionToken Subscribe<TMessage>(Action<TMessage> subscriber, Func<TMessage, bool> condition) where TMessage : class, ITinyMessage
        {
            var token = _bus.Subscribe(subscriber, condition);
            _factory.Log($"Subscriber '{typeof(TMessage).Name}' added to event bus.", LogLevel.Info, "EventBus");
            return token;
        }
        public void Publish<TMessage>(TMessage message) where TMessage : class, ITinyMessage
        {
            _bus.Publish(message);
            _proxy.PublishEvent(message);
        }
        public void UnSubscribe<TMessage>(TinyMessageSubscriptionToken token) where TMessage : class, ITinyMessage
        {
            _bus.UnSubscribe<TMessage>(token);
        }

        public void Publish(ITinyMessage message)
        {
            _bus.Publish(message);
            _proxy.PublishEvent(message);
        }

        public void RemoveEventProxy()
        {
            LoggerEventProxy.Instance.DeliveringEvent -= Instance_DeliveringEvent;
        }
    }
}