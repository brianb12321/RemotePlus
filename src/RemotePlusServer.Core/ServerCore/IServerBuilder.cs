using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusServer.Core.ServerCore
{
    /// <summary>
    /// Provides a way to plug in services and perform setup during server initialization.
    /// </summary>
    public interface IServerBuilder
    {
        IServerBuilder AddTask(Action task);
    }
}
