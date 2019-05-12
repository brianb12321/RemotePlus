using System;
using RemotePlusLibrary.Core.IOC;

namespace RemotePlusLibrary.Core.NodeStartup
{
    public interface INodeBuilder<HEAD> where HEAD : INodeBuilder<HEAD>
    {
        HEAD AddTask(Action task);
        INodeInitilizer Build();
    }
}