﻿using BetterLogger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyMessenger;

namespace RemotePlusLibrary.Core.EventSystem
{
    public class EventBus : IEventBus
    {
        private TinyMessengerHub _hub = new TinyMessengerHub();
        private ILogFactory _factory;

        public EventBus(ILogFactory factory)
        {
            _factory = factory;
            LoggerEventProxy.Instance.DeliveringEvent += Instance_DeliveringEvent;
        }

        private void Instance_DeliveringEvent(object sender, EventDeliveryEventArgs e)
        {
            _factory.Log($"Event raised '{e.EventType}'", LogLevel.Info, "EventBus");
        }

        public TinyMessageSubscriptionToken Subscribe<TMessage>(Action<TMessage> subscriber) where TMessage : class, ITinyMessage
        {
            var token = _hub.Subscribe(subscriber, LoggerEventProxy.Instance);
            _factory.Log($"Subscriber of type '{typeof(TMessage).Name}' added to event bus.", LogLevel.Info, "EventBus");
            return token;
        }
        public TinyMessageSubscriptionToken Subscribe<TMessage>(Action<TMessage> subscriber, Func<TMessage, bool> condition) where TMessage : class, ITinyMessage
        {
            var token = _hub.Subscribe(subscriber, condition, LoggerEventProxy.Instance);
            _factory.Log($"Subscriber '{typeof(TMessage).Name}' added to event bus.", LogLevel.Info, "EventBus");
            return token;
        }
        public void Publish<TMessage>(TMessage message) where TMessage : class, ITinyMessage
        {
            _hub.Publish(message);
        }
        public void Publish(ITinyMessage message)
        {
            _hub.Publish(message);
        }
        public void UnSubscribe<TMessage>(TinyMessageSubscriptionToken token) where TMessage : class, ITinyMessage
        {
            _hub.Unsubscribe<TMessage>(token);
        }

        public void RemoveEventProxy()
        {
            LoggerEventProxy.Instance.DeliveringEvent -= Instance_DeliveringEvent;
        }

        void IEventBus.PublishPrivate<TMessage>(TMessage message)
        {
            Publish(message);
        }

        public void PublishPrivate(ITinyMessage message)
        {
            Publish(message);
        }
    }
}