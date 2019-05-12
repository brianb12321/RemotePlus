using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Core.Behavior
{
    public class NetDataContractSerializerBehavior : IContractBehavior
    {
        public void Validate(ContractDescription contractDescription, ServiceEndpoint endpoint)
        {
            
        }

        public void ApplyDispatchBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint,
            DispatchRuntime dispatchRuntime)
        {
            foreach (var operations in contractDescription.Operations)
            {
                DataContractSerializerOperationBehavior behavior = operations.Behaviors.Find<DataContractSerializerOperationBehavior>();
                operations.Behaviors.Remove(behavior);
                operations.Behaviors.Add(new NetDataContractOperationBehavior(operations));
            }
        }

        public void ApplyClientBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint,
            ClientRuntime clientRuntime)
        {
            foreach (var operations in contractDescription.Operations)
            {
                DataContractSerializerOperationBehavior behavior = operations.Behaviors.Find<DataContractSerializerOperationBehavior>();
                operations.Behaviors.Remove(behavior);
                operations.Behaviors.Add(new NetDataContractOperationBehavior(operations));
            }
        }

        public void AddBindingParameters(ContractDescription contractDescription, ServiceEndpoint endpoint,
            BindingParameterCollection bindingParameters)
        {
            
        }
    }
}