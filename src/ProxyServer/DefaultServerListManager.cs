using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Discovery;

namespace ProxyServer
{
    public class DefaultServerListManager : IServerListManager
    {
        private object _lockObject = new object();
        private readonly List<SessionClient<IRemoteWithProxy>> _connectedServers = new List<SessionClient<IRemoteWithProxy>>();
        public IEnumerator<SessionClient<IRemoteWithProxy>> GetEnumerator()
        {
            return _connectedServers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => _connectedServers.Count;
        public void AddServer(SessionClient<IRemoteWithProxy> server)
        {
            lock (_lockObject)
            {
                _connectedServers.Add(server);
            }
        }

        public void RemoveServer(Guid id)
        {
            var server = GetByGuid(id);

            _connectedServers.Remove(server);
        }

        public void RemoveServer(SessionClient<IRemoteWithProxy> server)
        {
            _connectedServers.Remove(server);
        }

        public Guid[] GetAllServers()
        {
            return _connectedServers.Select(s => s.UniqueID).ToArray();
        }

        public SessionClient<IRemoteWithProxy> GetServerByChannel(IContextChannel channel)
        {
            return _connectedServers.FirstOrDefault(s => s.Channel == channel);
        }

        public SessionClient<IRemoteWithProxy> GetByGuid(Guid guid)
        {
            return _connectedServers.FirstOrDefault(s => s.UniqueID == guid);
        }

        public int IndexOf(SessionClient<IRemoteWithProxy> server)
        {
            return _connectedServers.IndexOf(server);
        }

        public SessionClient<IRemoteWithProxy> this[int serverPosition] => _connectedServers[serverPosition];
    }
}
