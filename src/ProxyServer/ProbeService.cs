using BetterLogger;
using ProxyServer;
using RemotePlusLibrary;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Discovery;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.ServiceArchitecture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace ProxyServer
{
    public class ProbeService : StandordService<ProxyServerRemoteImpl>
    {
        private Binding _clientBinding = null;
        private string _clientAddress = null;
        public ServiceEndpoint ClientEndpoint { get; private set; }
        public override void BuildHost()
        {
            base.BuildHost();
            ClientEndpoint = Host.AddServiceEndpoint(typeof(IProxyRemote), _clientBinding, _clientAddress);
        }
        public ProbeService(ProxyServerRemoteImpl singleTon, Binding serverBinding, Binding clientBinding, string proxyAddress, string proxyClientAddress) : base(typeof(IProxyServerRemote), singleTon, serverBinding, proxyAddress)
        {
            RemoteInterface = singleTon;
            _clientBinding = clientBinding;
            _clientAddress = proxyClientAddress;
        }
    }
}
