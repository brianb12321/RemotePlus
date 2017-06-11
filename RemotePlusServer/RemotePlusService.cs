using Logging;
using RemotePlusLibrary;
using RemotePlusLibrary.Extension.CommandSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusServer
{
    public class RemotePlusService<I> : NetNode where I : new()
    {
        public ServiceHost Host { get; private set; } = null;
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
            remove { Host.Faulted -= value; }
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
        public I Remote { get; } = new I();
        public Dictionary<string, CommandDelegate> Commands { get; } = new Dictionary<string, CommandDelegate>();
        public VariableManager Variables { get; set; }
        private RemotePlusService(I singleTon, int portNumber, Action<I> setupCallback)
        {
            Remote = singleTon;
            setupCallback?.Invoke(Remote);
            Host = new ServiceHost(Remote);
        }
        public void Start()
        {
            Host.Open();
        }
        public void Close()
        {
            Host.Close();
        }
        public static RemotePlusService<I> Create(I singleTon, int port, Action<string, OutputLevel> callback, Action<I> setupCallback)
        {
            RemotePlusService<I> temp;
            callback("Building endpoint URL.", OutputLevel.Debug);
            string url = $"net.tcp://0.0.0.0:{port}/Remote";
            callback($"URL built {url}", OutputLevel.Debug);
            callback("Creating server.", OutputLevel.Debug);
            callback("Publishing server events.", OutputLevel.Debug);
            temp = new RemotePlusService<I>(singleTon, port, setupCallback);
            callback("Changing url of endpoint 1.", OutputLevel.Debug);
            temp.Host.Description.Endpoints[0].Address = new EndpointAddress(url);
            return temp;
        }
    }
}
