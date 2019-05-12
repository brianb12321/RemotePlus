using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.ServiceArchitecture
{
    public interface IRemotePlusService<TRemoteInterface> : IRemotePlusCommunicationObject where TRemoteInterface : new()
    {
        bool IsSingleton { get; }
        /// <summary>
        /// The host object.
        /// </summary>
        ServiceHost Host { get; }
        List<IServiceBehavior> Behaviors { get; set; }
        Dictionary<Type, List<IContractBehavior>> ContractBehaviors { get; set; }
        /// <summary>
        /// Provides access to remote operations and variables.
        /// </summary>
        TRemoteInterface RemoteInterface { get; set; }
        void BuildHost();
        void AddEndpoint<TEndpoint>(TEndpoint endpoint, Binding binding, string endpointName, Action<TEndpoint> setupCallback);
    }
}