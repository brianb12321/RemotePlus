using RemotePlusLibrary;
using RemotePlusLibrary.Client;
using RemotePlusLibrary.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ProxyServer
{
    public class SessionClient<C> : Client<C> where C : IClient
    {
        public string SessionId { get; set; }
        protected SessionClient(ClientType ct) : base(ct)
        {
        }
        public static SessionClient<C> BuildSessionClient(ClientBuilder builder, C callback)
        {
            SessionClient<C> c = new SessionClient<C>(builder.ClientType)
            {
                FriendlyName = builder.FriendlyName,
                ClientCallback = callback,
                ExtraData = builder.ExtraData
            };
            return c;
        }
    }
}
