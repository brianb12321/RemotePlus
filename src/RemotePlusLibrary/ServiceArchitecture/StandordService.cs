using RemotePlusLibrary.SubSystem.Command;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace RemotePlusLibrary.ServiceArchitecture
{
    public abstract class StandordService<TInterface> : IRemotePlusService<TInterface> where TInterface : new()
    {
        public bool IsSingleton { get; private set; }
        public ServiceHost Host { get; protected set; }
        private Type _impl;
        private Type _contract;
        private Binding _binding;
        private string _address;
        private object _singleTon;
        public TInterface RemoteInterface { get; set; }
        public List<IServiceBehavior> Behaviors { get; set; } = new List<IServiceBehavior>();
        public Dictionary<Type, List<IEndpointBehavior>> EndpointBehaviors { get; set; }
        public Dictionary<Type, List<IContractBehavior>> ContractBehaviors { get; set; }

        public event EventHandler HostClosed;
        public event EventHandler HostClosing;
        public event EventHandler HostFaulted;
        public event EventHandler HostOpened;
        public event EventHandler HostOpening;
        public event EventHandler<UnknownMessageReceivedEventArgs> HostUnknownMessageReceived;
        public virtual void BuildHost()
        {
            if (Host == null)
            {
                if (_singleTon == null)
                {
                    Host = new ServiceHost(_impl);
                }
                else
                {
                    Host = new ServiceHost(_singleTon);
                }
            }
            Behaviors.ForEach(b => Host.Description.Behaviors.Add(b));
            Host.Closed += HostClosed;
            Host.Closing += HostClosing;
            Host.Faulted += HostFaulted;
            Host.Opened += HostOpened;
            Host.Opening += HostOpening;
            Host.UnknownMessageReceived += HostUnknownMessageReceived;
            Host.AddServiceEndpoint(_contract, _binding, _address);
            foreach (var endpoints in EndpointBehaviors)
            {
                foreach (var behavior in endpoints.Value)
                {
                    Host.Description.Endpoints.Find(endpoints.Key)?.Behaviors.Add(behavior);
                }
            }
            foreach (var contract in ContractBehaviors)
            {
                foreach (var behavior in contract.Value)
                {
                    Host.Description.Endpoints.Find(contract.Key)?.Contract.Behaviors.Add(behavior);
                }
            }
        }

        protected StandordService(Type contract, Type implementation, Binding binding, string address)
        {
            _contract = contract;
            _impl = implementation;
            _binding = binding;
            _address = address;
        }
        protected StandordService(Type contract, object singleTon, Binding binding, string address)
        {
            _contract = contract;
            IsSingleton = true;
            _singleTon = singleTon;
            _binding = binding;
            _address = address;
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