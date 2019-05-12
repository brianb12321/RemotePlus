using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Core.IOC;

namespace RemotePlusLibrary.Core.NodeStartup
{
    public interface INodeCoreStartup<TBuilder> where TBuilder : INodeBuilder<TBuilder>
    {
        void AddServices(IServiceCollection services);
        void InitializeNode(TBuilder builder);
        void PostInitializeNode(TBuilder builder);
    }
}