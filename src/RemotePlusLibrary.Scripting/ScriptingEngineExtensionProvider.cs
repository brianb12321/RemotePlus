using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Core.IOC;

namespace RemotePlusLibrary.Scripting
{
    public class ScriptingEngineExtensionProvider : IClientContextExtensionProvider
    {
        void IClientContextExtensionProvider.OnConnect(InstanceContext context, IServiceCollection services)
        {
            var engine = services.GetService<IScriptingEngine>();
            context.Extensions.Add(engine.CreateContext());
        }

        void IClientContextExtensionProvider.OnDisconnect(InstanceContext context, IServiceCollection services)
        {

        }
    }
}