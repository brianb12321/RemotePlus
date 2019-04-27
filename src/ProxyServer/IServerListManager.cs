using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Discovery;

namespace ProxyServer
{
    public interface IServerListManager : IEnumerable<SessionClient<IRemoteWithProxy>>
    {
        int Count { get; }
        void AddServer(SessionClient<IRemoteWithProxy> server);
        void RemoveServer(Guid id);
        void RemoveServer(SessionClient<IRemoteWithProxy> server);
        Guid[] GetAllServers();
        SessionClient<IRemoteWithProxy> GetServerByChannel(IContextChannel channel);
        SessionClient<IRemoteWithProxy> GetByGuid(Guid guid);
        int IndexOf(SessionClient<IRemoteWithProxy> server);
        SessionClient<IRemoteWithProxy> this[int serverPosition] { get; }
    }
}