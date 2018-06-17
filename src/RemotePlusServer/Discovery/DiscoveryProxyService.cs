using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Discovery;
using System.Xml;
using Logging;

namespace RemotePlusServer.Discovery
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class DiscoveryProxyService : DiscoveryProxy
    {
        // Stores endpoint discovery metadata of found services.
        Dictionary<EndpointAddress, EndpointDiscoveryMetadata> onlineServices;
        public DiscoveryProxyService()
        {
            onlineServices = new Dictionary<EndpointAddress, EndpointDiscoveryMetadata>();
        }
        void AddOnlineService(EndpointDiscoveryMetadata endpointDM)
        {
            lock(this.onlineServices)
            {
                this.onlineServices[endpointDM.Address] = endpointDM;
            }
            PrintDiscoveryMetadata(endpointDM, "Adding");
        }
        void RemoveOnlineService(EndpointDiscoveryMetadata endpointDM)
        {
            lock(this.onlineServices)
            {
                this.onlineServices.Remove(endpointDM.Address);
            }
            PrintDiscoveryMetadata(endpointDM, "Removing");
        }
        void MatchFromOnlineService(FindRequestContext findRequest)
        {
            lock(this.onlineServices)
            {
                foreach(EndpointDiscoveryMetadata med in onlineServices.Values)
                {
                    if(findRequest.Criteria.IsMatch(med))
                    {
                        findRequest.AddMatchingEndpoint(med);
                    }
                }
            }
        }
        EndpointDiscoveryMetadata MatchFromOnlineService(ResolveCriteria critera)
        {
            EndpointDiscoveryMetadata med = null;
            lock (this.onlineServices)
            {
                foreach(EndpointDiscoveryMetadata loopMed in onlineServices.Values)
                {
                    if(critera.Address == loopMed.Address)
                    {
                        med = loopMed;
                    }
                }
            }
            return med;
        }
        protected override IAsyncResult OnBeginOnlineAnnouncement(DiscoveryMessageSequence messageSequence, EndpointDiscoveryMetadata endpointDiscoveryMetadata, AsyncCallback callback, object state)
        {
            this.AddOnlineService(endpointDiscoveryMetadata);
            return new OnOnlineAnnouncementAsyncResult(callback, state);
        }
        protected override void OnEndOnlineAnnouncement(IAsyncResult result)
        {
            OnOnlineAnnouncementAsyncResult.End(result);
        }
        protected override IAsyncResult OnBeginOfflineAnnouncement(DiscoveryMessageSequence messageSequence, EndpointDiscoveryMetadata endpointDiscoveryMetadata, AsyncCallback callback, object state)
        {
            RemoveOnlineService(endpointDiscoveryMetadata);
            return new OnOfflineAnnouncementAsyncResult(callback, state);
        }
        protected override void OnEndOfflineAnnouncement(IAsyncResult result)
        {
            OnOfflineAnnouncementAsyncResult.End(result);
        }
        protected override IAsyncResult OnBeginFind(FindRequestContext findRequestContext, AsyncCallback callback, object state)
        {
            MatchFromOnlineService(findRequestContext);
            return new OnFindAsyncResult(callback, state);
        }
        protected override void OnEndFind(IAsyncResult result)
        {
            OnFindAsyncResult.End(result);
        }
        protected override IAsyncResult OnBeginResolve(ResolveCriteria resolveCriteria, AsyncCallback callback, object state)
        {
            return new OnResolveAsyncResult(MatchFromOnlineService(resolveCriteria), callback, state);
        }
        protected override EndpointDiscoveryMetadata OnEndResolve(IAsyncResult result)
        {
            return OnResolveAsyncResult.End(result);
        }

        private void PrintDiscoveryMetadata(EndpointDiscoveryMetadata endpointDM, string operation)
        {
            LogItem li = new LogItem(OutputLevel.Info, $"{operation} service of the following type from cache:", "Discovery");
            foreach(XmlQualifiedName contractName in endpointDM.ContractTypeNames)
            {
                li.Message = li.Message + ($"\n{contractName.ToString()}");
                break;
            }
            ServerManager.Logger.AddOutput(li);
            ServerManager.Logger.AddOutput("Operation Completed", OutputLevel.Info);
        }
    }
}
