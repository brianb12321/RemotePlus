using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace RemotePlusLibrary
{
    public interface IRemotePlusCommunicationObject
    {
        Dictionary<Type, List<IEndpointBehavior>> EndpointBehaviors { get; set; }
        /// <summary>
        /// Occures when the host object is closed.
        /// </summary>
        event EventHandler HostClosed;
        /// <summary>
        /// Occures when the host object is being closed.
        /// </summary>
        event EventHandler HostClosing;
        /// <summary>
        /// Occures when there was an error that has not been resolved, thus causing the object to be in a faulted state.
        /// </summary>
        event EventHandler HostFaulted;
        /// <summary>
        /// Occures when the object has started.
        /// </summary>
        event EventHandler HostOpened;
        /// <summary>
        /// Occures when the object is opening.
        /// </summary>
        event EventHandler HostOpening;
        /// <summary>
        /// Occurs when a unknown message has been received by the object.
        /// </summary>
        event EventHandler<UnknownMessageReceivedEventArgs> HostUnknownMessageReceived;
        void Start();
        void Close();
    }
}