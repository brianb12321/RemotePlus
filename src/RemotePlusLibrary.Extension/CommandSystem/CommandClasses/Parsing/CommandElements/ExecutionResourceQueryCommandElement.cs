using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.Extension.ResourceSystem;
using RemotePlusLibrary.Extension.ResourceSystem.ResourceTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.CommandSystem.CommandClasses.Parsing.CommandElements
{
    [DataContract]
    public class ExecutionResourceQueryCommandElement : ResourceQueryCommandElement
    {
        public ExecutionResourceQueryCommandElement(ResourceQuery rq) : base(rq)
        {
        }
        public override string ToString()
        {
            var resource = IOCContainer.GetService<IResourceManager>().GetResource<Resource>(Query);
            var environment = IOCContainer.GetService<ICommandEnvironment>();
            var result = environment.Execute(resource.ToString(), CommandExecutionMode.Client);
            return result.GetLatest().Output.ReturnData.ToString();
        }
    }
}
