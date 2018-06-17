using Logging;
using RemotePlusLibrary.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Discovery
{
    public class ProbeService<I> : RemotePlusService<I> where I : new()
    {
        public DiscoveryEndpoint Discovery { get; private set; }
        public AnnouncementEndpoint Announcement { get; private set; }
        public ProbeService(I singleTon, string probeAddress, string announcementAddress, Action<I> callback) : base(_ConnectionFactory.BuildBinding(), singleTon, callback)
        {
            Discovery = new DiscoveryEndpoint(_ConnectionFactory.BuildBinding(), new EndpointAddress(probeAddress));
            Discovery.IsSystemEndpoint = false;
            Announcement = new AnnouncementEndpoint(_ConnectionFactory.BuildBinding(), new EndpointAddress(announcementAddress));
            Host.AddServiceEndpoint(Discovery);
            Host.AddServiceEndpoint(Announcement);
        }
        public static ProbeService<I> CreateProbeService(I singleTon, int port, string probeEndpoint, string announcementEndpoint, Action<string, OutputLevel> callback, Action<I> setupCallback)
        {
            ProbeService<I> temp;
            callback?.Invoke("Building endpoint URL.", OutputLevel.Debug);
            string url = $"net.tcp://{Dns.GetHostName()}:{port}/{probeEndpoint}";
            string aurl = $"net.tcp://{Dns.GetHostName()}:{port}/{announcementEndpoint}";
            callback?.Invoke($"URL built {url}", OutputLevel.Debug);
            StringBuilder dataBuilder = new StringBuilder();
            callback?.Invoke(dataBuilder.ToString(), OutputLevel.Debug);
            temp = new ProbeService<I>(singleTon, url, aurl, setupCallback);
            return temp;
        }
    }
}
