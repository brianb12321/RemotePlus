using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Core.NodeStartup;

namespace RemotePlusClient.CommonUI
{
    public class ClientInitBuilder : IClientBuilder, INodeInitilizer
    {
        List<Action> _tasks = new List<Action>();
        public IClientBuilder AddTask(Action task)
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