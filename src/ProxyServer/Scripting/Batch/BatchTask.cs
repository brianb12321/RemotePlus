using RemotePlusLibrary.Discovery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyServer.Scripting.Batch
{
    public class BatchTask
    {
        private Action<SessionClient<IRemoteWithProxy>> _operation;
        public BatchTask(Action<SessionClient<IRemoteWithProxy>> operation)
        {
            _operation = operation;
        }
        public void run(SessionClient<IRemoteWithProxy> remote)
        {
            _operation?.Invoke(remote);
        }
    }
}
