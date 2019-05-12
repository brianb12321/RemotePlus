using System;
using System.Collections.Generic;
using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.Core.NodeStartup;

namespace ProxyServer
{
    public sealed class ServerTaskBuilder : IServerTaskBuilder, INodeInitilizer
    {
        List<Action> _tasks = new List<Action>();
        public IServerTaskBuilder AddTask(Action task)
        {
            _tasks.Add(task);
            return this;
        }

        public INodeInitilizer Build()
        {
            return this;
        }

        void INodeInitilizer.RunTasks()
        {
            foreach (Action task in _tasks)
            {
                task?.Invoke();
            }
        }
    }
}