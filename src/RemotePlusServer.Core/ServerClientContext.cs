using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Core;

namespace RemotePlusServer.Core
{
    public class ServerClientContext : IClientContext
    {
        private Client<RemoteClient> _client;
        private readonly IExtensionCollection<InstanceContext> _extensions;
        public ServerClientContext(Client<RemoteClient> client, RequestContext request, InstanceContext context, string username)
        {
            _client = client;
            MessageRequest = request;
            ClientUniqueID = client.UniqueID;
            _extensions = context.Extensions;
            Username = username;
        }
        public RequestContext MessageRequest { get; }
        public Guid ClientUniqueID { get; }
        public string Username { get; }

        public Client<TClient> GetClient<TClient>() where TClient : IClient
        {
            if (typeof(TClient) == typeof(RemoteClient))
            {
                return _client as Client<TClient>;
            }
            else throw new NotSupportedException("RemoteClient is the only supported client type.");
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