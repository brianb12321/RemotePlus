using Logging;
using ProxyServer;
using RemotePlusLibrary;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Discovery;
using RemotePlusLibrary.Extension.CommandSystem;
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
    public class ProbeService : IRemotePlusService<ProxyServerRemoteImpl>
    {
        public ServiceHost Host { get; private set; }

        public ProxyServerRemoteImpl RemoteInterface { get; private set; } = new ProxyServerRemoteImpl();

        public Dictionary<string, CommandDelegate> Commands { get; set; } = new Dictionary<string, CommandDelegate>();
        public VariableManager Variables { get; set; }

        public event EventHandler HostClosed
        {
            add { Host.Closed += value; }
            remove { Host.Closed -= value; }
        }
        public event EventHandler HostClosing
        {
            add { Host.Closing += value; }
            remove { Host.Closing -= value; }
        }
        public event EventHandler HostFaulted
        {
            add { Host.Faulted += value; }
            remove { Host.Faulted += value; }
        }
        public event EventHandler HostOpened
        {
            add { Host.Opened += value; }
            remove { Host.Opened -= value; }
        }
        public event EventHandler HostOpening
        {
            add { Host.Opening += value; }
            remove { Host.Opening -= value; }
        }
        public event EventHandler<UnknownMessageReceivedEventArgs> HostUnknownMessageReceived
        {
            add { Host.UnknownMessageReceived += value; }
            remove { Host.UnknownMessageReceived -= value; }
        }

        public void AddEndpoint<TEndpoint>(TEndpoint endpoint, Binding binding, string endpointName, Action<TEndpoint> setupCallback)
        {
            setupCallback?.Invoke(endpoint);
            Host.AddServiceEndpoint(typeof(TEndpoint), binding, endpointName);
        }
        /// <summary>
        /// Starts the server.
        /// </summary>
        public void Start()
        {
            Host.Open();
        }
        /// <summary>
        /// Closes the server.
        /// </summary>
        public void Close()
        {
            Host.Close();
        }
        public ServiceEndpoint ClientEndpoint { get; private set; }

        public ProbeService(Type serviceType, ProxyServerRemoteImpl singleTon, string proxyAddress, string proxyClientAddress, Action<ProxyServerRemoteImpl> callback)
        {
            Commands = new Dictionary<string, CommandDelegate>();
            RemoteInterface = singleTon;
            Host = new ServiceHost(RemoteInterface);
            Host.AddServiceEndpoint(typeof(IProxyServerRemote), _ConnectionFactory.BuildBinding(), proxyAddress);
            ClientEndpoint = Host.AddServiceEndpoint(typeof(IProxyRemote), _ConnectionFactory.BuildBinding(), proxyClientAddress);
        }
        public static IRemotePlusService<ProxyServerRemoteImpl> CreateProxyService(Type serviceType, ProxyServerRemoteImpl singleTon, int port, string proxyEndpoint, string proxyClientEndpoint, Action<string, OutputLevel> callback, Action<ProxyServerRemoteImpl> setupCallback)
        {
            ProbeService temp;
            callback?.Invoke("Building endpoint URL.", OutputLevel.Debug);
            string url = $"net.tcp://{Dns.GetHostName()}:{port}/{proxyEndpoint}";
            string curl = $"net.tcp://{Dns.GetHostName()}:{port}/{proxyClientEndpoint}";
            callback?.Invoke($"URL built {url}", OutputLevel.Debug);
            StringBuilder dataBuilder = new StringBuilder();
            callback?.Invoke(dataBuilder.ToString(), OutputLevel.Debug);
            temp = new ProbeService(serviceType, singleTon, url, curl, setupCallback);
            return temp;
        }
    }
}
