using RemotePlusLibrary;
using RemotePlusServer.Core.ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;

namespace RemotePlusServer
{
    public class ServiceCollection : IServiceCollection
    {
        public IServiceCollection AddSingleton<TService>(TService service)
        {
            IOCContainer.Provider.Bind<TService>().ToConstant(service);
            return this;
        }
    }
}
