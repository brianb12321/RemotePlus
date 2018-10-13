using RemotePlusLibrary;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.ServiceArchitecture;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace RemotePlusServer.Core
{
    /// <summary>
    /// Provides a container for the service.
    /// </summary>
    /// <typeparam name="I">The implementation of the service to use.</typeparam>
    public class ServerRemotePlusService : IRemotePlusService<ServerRemoteInterface>
    {
        public ServiceHost Host { get; }

        public Dictionary<string, CommandDelegate> Commands { get; set; } = new Dictionary<string, CommandDelegate>();
        public VariableManager Variables { get; set; }
        public ServerRemoteInterface RemoteInterface { get; set; }

        /// <summary>
        /// Creates a new instance of the <see cref="RemotePlusService{I}"/> class
        /// </summary>
        /// <param name="contractType"></param>
        /// <param name="singleTon">The instance of the service implementation.</param>
        /// <param name="binding"></param>
        /// <param name="address"></param>
        /// <param name="portNumber">The port number to use for listening.</param>
        public ServerRemotePlusService(Type contractType, object singleTon, Binding binding, string address)
        {
            Commands = new Dictionary<string, CommandDelegate>();
            Host = new ServiceHost(singleTon);
            Host.AddServiceEndpoint(contractType, binding, address);
        }

        public ServerRemotePlusService(Binding b, object singleTon)
        {
            Commands = new Dictionary<string, CommandDelegate>();
            Host = new ServiceHost(singleTon);
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
    }
}