using RemotePlusLibrary.Discovery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyServer.Scripting
{
    public class BatchJob
    {
        private List<SessionClient<IRemoteWithProxy>> servers = new List<SessionClient<IRemoteWithProxy>>();
        public void Addserver(int serverID)
        {
            
        }
    }
}