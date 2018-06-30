using Logging;
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

        /// <summary>
        /// Creates a new instance of the <see cref="FileTransferService"/> class
        /// </summary>
        /// <param name="singleTon">The instance of the service implementation.</param>
        /// <param name="portNumber">The port number to use for listening.</param>
        /// <param name="setupCallback">The function to call when setting up the service implementation.</param>
        protected FileTransferService(Type contractType, RemoteImpl singleTon, Binding binding, string address, Action<RemoteImpl> setupCallback)
        {
            Commands = new Dictionary<string, CommandDelegate>();
            Remote = singleTon;
            setupCallback?.Invoke(Remote);
            Host = new ServiceHost(Remote);
            Host.AddServiceEndpoint(contractType, binding, address);
        }
        protected FileTransferService(Binding b, RemoteImpl singleTon, Action<RemoteImpl> setupCallback)
        {
            Commands = new Dictionary<string, CommandDelegate>();
            Remote = singleTon;
            setupCallback?.Invoke(Remote);
            Host = new ServiceHost(Remote);
        }
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
        /// <summary>
        /// Creates a new server object that can be opened.
        /// </summary>
        /// <param name="singleTon">The instance of a service implementation</param>
        /// <param name="port">The port number to use for listening.</param>
        /// <param name="callback">The callback to use when an event occures for logging.</param>
        /// <param name="setupCallback">The callback to use when setting up the service implementation.</param>
        /// <returns></returns>
        public static IRemotePlusService<FileTransferServciceInterface> CreateNotSingle(Type contractType, int port, string defaultEndpoint, Action<string, OutputLevel> callback)
        {
            FileTransferService temp;
            callback?.Invoke("Building endpoint URL.", OutputLevel.Debug);
            string url = $"net.tcp://0.0.0.0:{port}/{defaultEndpoint}";
            callback?.Invoke($"URL built {url}", OutputLevel.Debug);
            callback?.Invoke("Creating server.", OutputLevel.Debug);
            callback?.Invoke("Publishing server events.", OutputLevel.Debug);
            NetTcpBinding binding = _ConnectionFactory.BuildBinding();
            StringBuilder dataBuilder = new StringBuilder();
            dataBuilder.AppendLine("Binding configurations:");
            dataBuilder.AppendLine();
            dataBuilder.AppendLine($"MaxBufferPoolSize: {binding.MaxBufferPoolSize}");
            dataBuilder.AppendLine($"MaxBufferSize: {binding.MaxBufferSize}");
            dataBuilder.AppendLine($"MaxReceivedMessageSize: {binding.MaxReceivedMessageSize}");
            callback?.Invoke(dataBuilder.ToString(), OutputLevel.Debug);
            temp = new FileTransferService(contractType, binding, url);
            return temp;
        }
        public static IRemotePlusService<FileTransferServciceInterface> CreateNotSingle(Type contractType, int port, Binding binding, string defaultEndpoint, Action<string, OutputLevel> callback)
        {
            FileTransferService temp;
            callback?.Invoke("Building endpoint URL.", OutputLevel.Debug);
            string url = $"{binding.Scheme}://0.0.0.0:{port}/{defaultEndpoint}";
            callback?.Invoke($"URL built {url}", OutputLevel.Debug);
            callback?.Invoke("Creating server.", OutputLevel.Debug);
            callback?.Invoke("Publishing server events.", OutputLevel.Debug);
            StringBuilder dataBuilder = new StringBuilder();
            dataBuilder.AppendLine("Binding configurations:");
            dataBuilder.AppendLine();
            callback?.Invoke(dataBuilder.ToString(), OutputLevel.Debug);
            temp = new FileTransferService(contractType, binding, url);
            return temp;
        }
    }
}
