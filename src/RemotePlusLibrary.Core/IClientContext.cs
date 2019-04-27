using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace RemotePlusLibrary.Core
{
    /// <summary>
    /// Represents the current communication channel.
    /// </summary>
    /// <typeparam name="TClient">The type of client that is opened.</typeparam>
    public interface IClientContext
    {
        /// <summary>
        /// Contains the request message of the current operation.
        /// </summary>
        RequestContext MessageRequest { get; }
        Guid ClientUniqueID { get; }
        string Username { get; }
        Client<TClient> GetClient<TClient>() where TClient : IClient;
        T GetExtension<T>() where T : IExtension<InstanceContext>;
        void AddExtension<T>(T extension) where T : IExtension<InstanceContext>;
    }
}