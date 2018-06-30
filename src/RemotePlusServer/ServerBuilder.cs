using RemotePlusServer.Core.ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusServer
{
    public class ServerBuilder : IServerBuilder
    {
        List<Action> _tasks = new List<Action>();
        public IServerBuilder AddTask(Action task)
        {
            _tasks.Add(task);
            return this;
        }
        public void Run()
        {
            foreach(Action task in _tasks)
            {
                task?.Invoke();
            }
        }
    }
}
