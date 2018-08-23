using RemotePlusLibrary.Core.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusServer.Core;
using RemotePlusServer.Core.ServerCore;
using RemotePlusLibrary;
using RemotePlusLibrary.Configuration;
using RemotePlusLibrary.Security.AccountSystem;

namespace RemotePlusServer.UnitTests
{
    public class MockStartup : IServerCoreStartup
    {
        public void AddServices(IServiceCollection services)
        {
            services.AddSingletonNamed<IConfigurationDataAccess, MockConfigAccess>("BinaryDataAccess")
                .UseAuthentication<MockAuthentication, RoleManager>();
        }

        public void InitializeServer(IServerBuilder builder)
        {
            throw new NotImplementedException();
        }
    }
}
