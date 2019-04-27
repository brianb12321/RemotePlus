using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Contracts;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Discovery;

namespace ProxyServer
{
    public class ProxyClientContext : IClientContext
    {
        private Client<IRemoteClient> _client;
        private SessionClient<IRemoteWithProxy> _selectedClient;
        private readonly IExtensionCollection<InstanceContext> _extensions;
        public string Username { get; }
        public ProxyClientContext(Client<IRemoteClient> client, SessionClient<IRemoteWithProxy> selectedClient, RequestContext request, InstanceContext context, string username)
        {
            _client = client;
            _selectedClient = selectedClient;
            MessageRequest = request;
            ClientUniqueID = _client.UniqueID;
            _extensions = context.Extensions;
            Username = username;
        }
        public RequestContext MessageRequest { get; }
        public Guid ClientUniqueID { get; }

        public Client<TClient> GetClient<TClient>() where TClient : IClient
        {
            if (typeof(TClient) == typeof(IRemoteClient))
            {
                return _client as Client<TClient>;
            }

            if (typeof(TClient) == typeof(IRemoteWithProxy))
            {
                return _selectedClient as SessionClient<TClient>;
            }

            throw new NotSupportedException("IRemoteClient and IRemoteWithProxy are the only client types supported");
        }

        public T GetExtension<T>() where T : IExtension<InstanceContext>
        {
            return _extensions.Find<T>();
        }

        public void AddExtension<T>(T extension) where T : IExtension<InstanceContext>
        {
            _extensions.Add(extension);
        }
    }
}