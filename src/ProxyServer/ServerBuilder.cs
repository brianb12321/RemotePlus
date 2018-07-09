using System;
using System.Collections.Generic;
using RemotePlusLibrary.Core.IOC;

namespace ProxyServer
{
    public class ServerBuilder : IServerBuilder, IServerInitilizer
    {
        List<Action> _tasks = new List<Action>();
        public IServerBuilder AddTask(Action task)
        {
            _tasks.Add(task);
            return this;
        }

        public IServerInitilizer Build()
        {
            return this;
        }

        void IServerInitilizer.RunTasks()
        {
            foreach (Action task in _tasks)
            {
                task?.Invoke();
            }
        }
    }
}