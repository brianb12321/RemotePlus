using System;

namespace RemotePlusLibrary.Core.EventSystem
{
    public class EventDeliveryEventArgs : EventArgs
    {
        public string EventType { get; private set; }
        public EventDeliveryEventArgs(string eventType)
        {
            EventType = eventType;
        }
    }
}