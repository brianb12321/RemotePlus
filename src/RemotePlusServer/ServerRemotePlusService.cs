using BetterLogger;
using RemotePlusLibrary;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusServer;
using RemotePlusServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary
{
    /// <summary>
    /// Provides a container for the service.
    /// </summary>
    /// <typeparam name="I">The implementation of the service to use.</typeparam>
    public class ServerRemotePlusService : IRemotePlusService<ServerRemoteInterface>
    {
        public ServiceHost Host { get; }

        public RemoteImpl Remote { get; }

        public Dictionary<string, CommandDelegate> Commands { get; set; } = new Dictionary<string, CommandDelegate>();
        public VariableManager Variables { get; set; }
        public ServerRemoteInterface RemoteInterface { get; } = new ServerRemoteInterface();

        /// <summary>
        /// Creates a new instance of the <see cref="RemotePlusService{I}"/> class
        /// </summary>
        /// <param name="contractType"></param>
        /// <param name="singleTon">The instance of the service implementation.</param>
        /// <param name="binding"></param>
        /// <param name="address"></param>
        /// <param name="portNumber">The port number to use for listening.</param>
        /// <param name="setupCallback">The function to call when setting up the service implementation.</param>
        protected ServerRemotePlusService(Type contractType, RemoteImpl singleTon, Binding binding, string address, Action<RemoteImpl> setupCallback)
        {
            Commands = new Dictionary<string, CommandDelegate>();
            Remote = singleTon;
            setupCallback?.Invoke(Remote);
            Host = new ServiceHost(Remote);
            Host.AddServiceEndpoint(contractType, binding, address);
        }

        protected ServerRemotePlusService(Binding b, RemoteImpl singleTon, Action<RemoteImpl> setupCallback)
        {
            Commands = new Dictionary<string, CommandDelegate>();
            Remote = singleTon;
            setupCallback?.Invoke(Remote);
            Host = new ServiceHost(Remote);
        }
        protected ServerRemotePlusService(Type contractType, Binding binding, string address)
        {
            Commands = new Dictionary<string, CommandDelegate>();
            Host = new ServiceHost(typeof(RemoteImpl));
            Host.AddServiceEndpoint(contractType, binding, address);
        }
        public ServerRemotePlusService()
        {
            Commands = new Dictionary<string, CommandDelegate>();
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
        public static IRemotePlusService<ServerRemoteInterface> Create(Type contractType, RemoteImpl singleTon, int port, string defaultEndpoint, Action<string, LogLevel> callback, Action<RemoteImpl> setupCallback)
        {
            ServerRemotePlusService temp;
            callback?.Invoke("Building endpoint URL.", LogLevel.Debug);
            string url = $"net.tcp://{Dns.GetHostName()}:{port}/{defaultEndpoint}";
            callback?.Invoke($"URL built {url}", LogLevel.Debug);
            callback?.Invoke("Creating server.", LogLevel.Debug);
            callback?.Invoke("Publishing server events.", LogLevel.Debug);
            NetTcpBinding binding = _ConnectionFactory.BuildBinding();
            StringBuilder dataBuilder = new StringBuilder();
            dataBuilder.AppendLine("Binding configurations:");
            dataBuilder.AppendLine();
            dataBuilder.AppendLine($"MaxBufferPoolSize: {binding.MaxBufferPoolSize}");
            dataBuilder.AppendLine($"MaxBufferSize: {binding.MaxBufferSize}");
            dataBuilder.AppendLine($"MaxReceivedMessageSize: {binding.MaxReceivedMessageSize}");
            callback?.Invoke(dataBuilder.ToString(), LogLevel.Debug);
            temp = new ServerRemotePlusService(contractType, singleTon, binding, url, setupCallback);
            singleTon.SetRemoteInterface(temp.RemoteInterface);
            return temp;
        }
        public static IRemotePlusService<ServerRemoteInterface> CreateNotSingle(Type contractType, int port, string defaultEndpoint, Action<string, LogLevel> callback)
        {
            ServerRemotePlusService temp;
            callback?.Invoke("Building endpoint URL.", LogLevel.Debug);
            string url = $"net.tcp://0.0.0.0:{port}/{defaultEndpoint}";
            callback?.Invoke($"URL built {url}", LogLevel.Debug);
            callback?.Invoke("Creating server.", LogLevel.Debug);
            callback?.Invoke("Publishing server events.", LogLevel.Debug);
            NetTcpBinding binding = _ConnectionFactory.BuildBinding();
            StringBuilder dataBuilder = new StringBuilder();
            dataBuilder.AppendLine("Binding configurations:");
            dataBuilder.AppendLine();
            dataBuilder.AppendLine($"MaxBufferPoolSize: {binding.MaxBufferPoolSize}");
            dataBuilder.AppendLine($"MaxBufferSize: {binding.MaxBufferSize}");
            dataBuilder.AppendLine($"MaxReceivedMessageSize: {binding.MaxReceivedMessageSize}");
            callback?.Invoke(dataBuilder.ToString(), LogLevel.Debug);
            temp = new ServerRemotePlusService(contractType, binding, url);
            return temp;
        }
        public static IRemotePlusService<ServerRemoteInterface> CreateNotSingle(Type contractType, int port, Binding binding, string defaultEndpoint, Action<string, LogLevel> callback)
        {
            ServerRemotePlusService temp;
            callback?.Invoke("Building endpoint URL.", LogLevel.Debug);
            string url = $"{binding.Scheme}://0.0.0.0:{port}/{defaultEndpoint}";
            callback?.Invoke($"URL built {url}", LogLevel.Debug);
            callback?.Invoke("Creating server.", LogLevel.Debug);
            callback?.Invoke("Publishing server events.", LogLevel.Debug);
            StringBuilder dataBuilder = new StringBuilder();
            dataBuilder.AppendLine("Binding configurations:");
            dataBuilder.AppendLine();
            callback?.Invoke(dataBuilder.ToString(), LogLevel.Debug);
            temp = new ServerRemotePlusService(contractType, binding, url);
            return temp;
        }
    }
}