using RemotePlusLibrary.Extension.CommandSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary
{
    public interface IRemotePlusService<TRemoteInterface> where TRemoteInterface : new()
    {
        /// <summary>
        /// The host object.
        /// </summary>
        ServiceHost Host { get; }
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
        /// The commands that are loaded on the server.
        /// </summary>
        Dictionary<string, CommandDelegate> Commands { get; set; }
        /// <summary>
        /// Provides access to remote operations and variables.
        /// </summary>
        TRemoteInterface RemoteInterface { get; }
        /// <summary>
        /// The variables that are defined on the server.
        /// </summary>
        VariableManager Variables { get; set; }
        void AddEndpoint<TEndpoint>(TEndpoint endpoint, Binding binding, string endpointName, Action<TEndpoint> setupCallback);
        void Start();
        void Close();
    }
}