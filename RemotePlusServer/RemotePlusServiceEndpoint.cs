using Logging;
using RemotePlusLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusServer
{
    public class RemotePlusServiceEndpoint<E> : NetNode where E : new()
    {
        public E Remote { get; private set; } = new E();
        public string Url { get; private set; }
        public Dictionary<string, CommandDelgate> Commands { get; } = new Dictionary<string, CommandDelgate>();
        public VariableManager Variables { get; set; }
        private RemotePlusServiceEndpoint(string url, E singleTon, Action<E> setupCallback)
        {
            Remote = singleTon;
            Url = url;
            setupCallback?.Invoke(Remote);
        }
        public static RemotePlusServiceEndpoint<E> Create(string name, E singleTon, int port, Action<E> setupCallback, Action<string, OutputLevel> callback)
        {
            RemotePlusServiceEndpoint<E> temp;
            callback("Building endpoint URL.", OutputLevel.Debug);
            string url = $"net.tcp://0.0.0.0:{port}/{name}";
            callback($"URL built {url}", OutputLevel.Debug);
            callback("Creating server.", OutputLevel.Debug);
            callback("Publishing server events.", OutputLevel.Debug);
            temp = new RemotePlusServiceEndpoint<E>(url, singleTon, setupCallback);
            callback("Changing url of endpoint 1.", OutputLevel.Debug);
            return temp;
        }
    }
}
