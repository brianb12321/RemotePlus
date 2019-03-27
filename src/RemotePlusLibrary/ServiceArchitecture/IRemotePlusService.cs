﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.ServiceArchitecture
{
    public interface IRemotePlusService<TRemoteInterface> where TRemoteInterface : new()
    {
        /// <summary>
        /// The host object.
        /// </summary>
        ServiceHost Host { get; }
        List<IServiceBehavior> Behaviors { get; }
        /// <summary>
        /// Occures when the host object is closed.
        /// </summary>
        event EventHandler HostClosed;
        /// <summary>
        /// Occures when the host object is being closed.
        /// </summary>
        event EventHandler HostClosing;
        /// <summary>
        /// Occures when there was an error that has not been resolved, thus causing the server to be in a faulted state.
        /// </summary>
        event EventHandler HostFaulted;
        /// <summary>
        /// Occures when the server has started.
        /// </summary>
        event EventHandler HostOpened;
        /// <summary>
        /// Occures when the server is opening.
        /// </summary>
        event EventHandler HostOpening;
        /// <summary>
        /// Occures when a unkown message has been received by the server.
        /// </summary>
        event EventHandler<UnknownMessageReceivedEventArgs> HostUnknownMessageReceived;
        /// <summary>
        /// Provides access to remote operations and variables.
        /// </summary>
        TRemoteInterface RemoteInterface { get; set; }
        void BuildHost();
        void AddEndpoint<TEndpoint>(TEndpoint endpoint, Binding binding, string endpointName, Action<TEndpoint> setupCallback);
        void Start();
        void Close();
    }
}