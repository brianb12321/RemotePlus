using RemotePlusClient.CommonUI.Connection;
using RemotePlusLibrary;
using RemotePlusLibrary.Contracts;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Discovery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusClient
{
    public class DefaultConnectionManager : IConnectionManager
    {
        CommonUI.ConnectionClients.ServiceClient defaultClient = null;
        CommonUI.ConnectionClients.ProxyClient proxyClient = null;

        public object ClientCallback { get; set; }

        public void Close()
        {
            defaultClient?.Close();
            proxyClient?.Close();
        }

        public IProxyRemote GetProxyRemote()
        {
            return proxyClient;
        }

        public IRemote GetRemote()
        {
            if (defaultClient != null) return defaultClient;
            else if (proxyClient != null) return proxyClient;
            else return null;
        }

        public CommunicationState GetState()
        {
            if (defaultClient != null) return defaultClient.State;
            else if (proxyClient != null) return proxyClient.State;
            else return CommunicationState.Created;
        }

        public void Open(Connection conn)
        {
            if(conn.ConnectAsProxy)
            {
                proxyClient = new CommonUI.ConnectionClients.ProxyClient(ClientCallback, _ConnectionFactory.BuildBinding(), new EndpointAddress(conn.WholeAddress));
            }
            else
            {
                defaultClient = new CommonUI.ConnectionClients.ServiceClient(ClientCallback, _ConnectionFactory.BuildBinding(), new EndpointAddress(conn.WholeAddress));
            }
        }

        public void Register(RegisterationObject obj)
        {
            if (defaultClient != null) defaultClient.Register(obj);
            else if (proxyClient != null) proxyClient.ProxyRegister();
        }
    }
}