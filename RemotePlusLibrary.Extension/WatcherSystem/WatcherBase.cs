using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.WatcherSystem
{
    public abstract class WatcherBase
    {
        protected Thread UnderlyingThread { get; set; }
        public WatcherDetails Details { get; protected set; }
        public WatcherStatus Status { get; private set; }
        protected abstract void DoWork(object args);
        protected WatcherBase(WatcherDetails details)
        {
            Details = details;
            Status = WatcherStatus.Off;
        }
        public void Start(object args)
        {
            UnderlyingThread = new Thread(DoWork);
            UnderlyingThread.Start(args);
            Status = WatcherStatus.Running;
        }
        protected abstract void Stop();
        public void StopWatcher()
        {
            Status = WatcherStatus.ShuttingDown;
            Stop();
            Status = WatcherStatus.Off;
        }
        protected void SetFatal()
        {
            Status = WatcherStatus.Fatal;
        }
    }
}