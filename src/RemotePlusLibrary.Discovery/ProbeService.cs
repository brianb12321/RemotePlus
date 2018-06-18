using Logging;
using RemotePlusLibrary.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Discovery
{
    public class ProbeService<I> : RemotePlusService<I> where I : new()
    {
        public ServiceEndpoint ClientEndpoint { get; private set; }
        public ProbeService(Type serviceType, I singleTon, string proxyAddress, string proxyClientAddress, Action<I> callback) : base(serviceType, singleTon, _ConnectionFactory.BuildBinding(), proxyAddress, callback)
        {
            ClientEndpoint = Host.AddServiceEndpoint(typeof(IProxyRemote), _ConnectionFactory.BuildBinding(), proxyClientAddress);
        }
        public static ProbeService<I> CreateProxyService(Type serviceType, I singleTon, int port, string proxyEndpoint, string proxyClientEndpoint, Action<string, OutputLevel> callback, Action<I> setupCallback)
        {
            ProbeService<I> temp;
            callback?.Invoke("Building endpoint URL.", OutputLevel.Debug);
            string url = $"net.tcp://{Dns.GetHostName()}:{port}/{proxyEndpoint}";
            string curl = $"net.tcp://{Dns.GetHostName()}:{port}/{proxyClientEndpoint}";
            callback?.Invoke($"URL built {url}", OutputLevel.Debug);
            StringBuilder dataBuilder = new StringBuilder();
            callback?.Invoke(dataBuilder.ToString(), OutputLevel.Debug);
            temp = new ProbeService<I>(serviceType, singleTon, url, curl, setupCallback);
            return temp;
        }
    }
}
