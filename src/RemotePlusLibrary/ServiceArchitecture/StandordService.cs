using RemotePlusLibrary.Extension.CommandSystem;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace RemotePlusLibrary.ServiceArchitecture
{
    public abstract class StandordService<TInterface> : IRemotePlusService<TInterface> where TInterface : new()
    {
        public ServiceHost Host { get; protected set; }

        public Dictionary<string, CommandDelegate> Commands { get; set; } = new Dictionary<string, CommandDelegate>();
        public VariableManager Variables { get; set; }
        public TInterface RemoteInterface { get; set; }
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
        protected StandordService(Type contract, Type implementation, Binding binding, string address)
        {
            Commands = new Dictionary<string, CommandDelegate>();
            Host = new ServiceHost(implementation);
            Host.AddServiceEndpoint(contract, binding, address);
        }
        protected StandordService(Type contract, object singleTon, Binding binding, string address)
        {
            Commands = new Dictionary<string, CommandDelegate>();
            Host = new ServiceHost(singleTon);
            Host.AddServiceEndpoint(contract, binding, address);
        }
        public virtual void AddEndpoint<TEndpoint>(TEndpoint endpoint, Binding binding, string endpointName, Action<TEndpoint> setupCallback)
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