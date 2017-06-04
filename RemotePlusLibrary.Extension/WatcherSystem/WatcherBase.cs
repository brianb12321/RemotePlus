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
        public WatcherOperationStatus State { get; private set; }
        protected Task UnderlyingTask { get; set; }
        public WatcherDetails Details { get; protected set; }
        public WatcherStatus Status { get; private set; }
        protected abstract void DoWork(object args);
        protected abstract void OnFail(AggregateException exception);
        protected WatcherBase(WatcherDetails details)
        {
            Details = details;
            Status = WatcherStatus.Off;
        }
        public void Start(object args)
        {
            UnderlyingTask = new Task(new Action<object>(DoWork), args);
            UnderlyingTask.ContinueWith(t => OnFail(t.Exception), TaskContinuationOptions.OnlyOnFaulted);
            UnderlyingTask.Start();
            Status = WatcherStatus.Running;
        }
        protected abstract void Stop();
        public void StopWatcher(WatcherOperationStatus status)
        {
            Status = WatcherStatus.ShuttingDown;
            State = status;
            Stop();
            Status = WatcherStatus.Off;
        }
        protected void SetFatal()
        {
            State = new WatcherOperationStatus()
            {
                Success = false
            };
            Status = WatcherStatus.Fatal;
        }
    }
}