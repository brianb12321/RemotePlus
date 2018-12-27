using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Core.IOC
{
    public interface IServerCoreStartup
    {
        void InitializeServer(IServerBuilder builder);
        void AddServices(IServiceCollection services);
        void PostInitializeServer(IServerBuilder builder);
    }
}