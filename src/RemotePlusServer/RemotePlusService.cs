using Logging;
using RemotePlusLibrary;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Extension.CommandSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusServer
{
    /// <summary>
    /// Provides a container for the service.
    /// </summary>
    /// <typeparam name="I">The implementation of the service to use.</typeparam>
    public class RemotePlusService<I> : NetNode where I : new()
    {
        /// <summary>
        /// The host object.
        /// </summary>
        public ServiceHost Host { get; private set; } = null;
        /// <summary>
        /// Occures when the host object is closed.
        /// </summary>
        public event EventHandler HostClosed
        {
            add { Host.Closed += value; }
            remove { Host.Closed -= value; }
        }
        /// <summary>
        /// Occures when the host object is being closed.
        /// </summary>
        public event EventHandler HostClosing
        {
            add { Host.Closing += value; }
            remove { Host.Closing -= value; }
        }
        /// <summary>
        /// Occures when there was an error that has not been resolved, thus causing the server to be in a faulted state.
        /// </summary>
        public event EventHandler HostFaulted
        {
            add { Host.Faulted += value; }
            remove { Host.Faulted -= value; }
        }
        /// <summary>
        /// Occures when the server has started.
        /// </summary>
        public event EventHandler HostOpened
        {
            add { Host.Opened += value; }
            remove { Host.Opened -= value; }
        }
        /// <summary>
        /// Occures when the server is opening.
        /// </summary>
        public event EventHandler HostOpening
        {
            add { Host.Opening += value; }
            remove { Host.Opening -= value; }
        }
        /// <summary>
        /// Occures when a unkown message has been received by the server.
        /// </summary>
        public event EventHandler<UnknownMessageReceivedEventArgs> HostUnknownMessageReceived
        {
            add { Host.UnknownMessageReceived += value; }
            remove { Host.UnknownMessageReceived -= value; }
        }
        /// <summary>
        /// The remote implemtation of the service.
        /// </summary>
        public I Remote { get; } = new I();
        /// <summary>
        /// The commands that are loaded on the server.
        /// </summary>
        public Dictionary<string, CommandDelegate> Commands { get; } = new Dictionary<string, CommandDelegate>();
        /// <summary>
        /// The variables that are defined on the server.
        /// </summary>
        public VariableManager Variables { get; set; }
        /// <summary>
        /// Creates a new instance of the <see cref="RemotePlusService{I}"/> class
        /// </summary>
        /// <param name="singleTon">The instance of the service implementation.</param>
        /// <param name="portNumber">The port number to use for listening.</param>
        /// <param name="setupCallback">The function to call when setting up the service implementation.</param>
        private RemotePlusService(I singleTon, NetTcpBinding binding, string address, Action<I> setupCallback)
        {
            Remote = singleTon;
            setupCallback?.Invoke(Remote);
            Host = new ServiceHost(Remote);
            Host.AddServiceEndpoint(typeof(IRemote), binding, address);
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
        public static RemotePlusService<I> Create(I singleTon, int port, Action<string, OutputLevel> callback, Action<I> setupCallback)
        {
            RemotePlusService<I> temp;
            callback("Building endpoint URL.", OutputLevel.Debug);
            string url = $"net.tcp://0.0.0.0:{port}/Remote";
            callback($"URL built {url}", OutputLevel.Debug);
            callback("Creating server.", OutputLevel.Debug);
            callback("Publishing server events.", OutputLevel.Debug);
            NetTcpBinding binding = _ConnectionFactory.BuildBinding();
            StringBuilder dataBuilder = new StringBuilder();
            dataBuilder.AppendLine("Binding configurations:");
            dataBuilder.AppendLine();
            dataBuilder.AppendLine($"MaxBufferPoolSize: {binding.MaxBufferPoolSize}");
            dataBuilder.AppendLine($"MaxBufferSize: {binding.MaxBufferSize}");
            dataBuilder.AppendLine($"MaxReceivedMessageSize: {binding.MaxReceivedMessageSize}");
            callback(dataBuilder.ToString(), OutputLevel.Debug);
            temp = new RemotePlusService<I>(singleTon, binding, url, setupCallback);
            return temp;
        }
    }
}