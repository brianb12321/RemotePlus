using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyMessenger;

namespace RemotePlusLibrary.Extension.EventSystem
{
    public class LoggerEventProxy : ITinyMessageProxy
    {
        public event EventHandler<EventDeliveryEventArgs> DeliveringEvent;
        static LoggerEventProxy()
        {
        }

        /// <summary>
        /// Singleton instance of the proxy.
        /// </summary>
        public static LoggerEventProxy Instance { get; } = new LoggerEventProxy();

        private LoggerEventProxy()
        {
        }

        public void Deliver(ITinyMessage message, ITinyMessageSubscription subscription)
        {
            DeliveringEvent?.Invoke(this, new EventDeliveryEventArgs(message.GetType().Name));
            subscription.Deliver(message);
        }
    }
}
