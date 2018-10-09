using BetterLogger;
using RemotePlusLibrary;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusServer
{
    public class FileTransferService : IRemotePlusService<FileTransferServciceInterface>
    {
        public ServiceHost Host { get; private set; }

        public RemoteImpl Remote { get; private set; }

        public Dictionary<string, CommandDelegate> Commands { get; set; } = new Dictionary<string, CommandDelegate>();
        public VariableManager Variables { get; set; }
        public FileTransferServciceInterface RemoteInterface { get; private set; }
        private FileTransferService(Type contractType, Binding binding, string address)
        {
            Commands = new Dictionary<string, CommandDelegate>();
            Host = new ServiceHost(typeof(FileTransferServiceImpl));
            Host.AddServiceEndpoint(contractType, binding, address);
        }

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
        public static IRemotePlusService<FileTransferServciceInterface> CreateNotSingle(Type contractType, int port, Binding binding, string defaultEndpoint, Action<string, LogLevel> callback)
        {
            FileTransferService temp;
            callback?.Invoke("Building endpoint URL.", LogLevel.Debug);
            string url = $"{binding.Scheme}://0.0.0.0:{port}/{defaultEndpoint}";
            callback?.Invoke($"URL built {url}", LogLevel.Debug);
            callback?.Invoke("Creating server.", LogLevel.Debug);
            callback?.Invoke("Publishing server events.", LogLevel.Debug);
            StringBuilder dataBuilder = new StringBuilder();
            dataBuilder.AppendLine("Binding configurations:");
            dataBuilder.AppendLine();
            callback?.Invoke(dataBuilder.ToString(), LogLevel.Debug);
            temp = new FileTransferService(contractType, binding, url);
            return temp;
        }
    }
}
