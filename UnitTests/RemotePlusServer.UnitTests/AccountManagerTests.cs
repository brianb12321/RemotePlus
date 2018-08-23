using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using RemotePlusServer.Core;
using RemotePlusLibrary.Security.AccountSystem;

namespace RemotePlusServer.UnitTests
{
    public class AccountManagerTests
    {
        [Theory]
        [InlineData("admin", "password")]
        [InlineData("abc123", "doereme")]
        [InlineData("Admin", "password")]
        [InlineData("admin", "Password")]
        public void AttemptLogin_LoginFail(string username, string password)
        {
            MockStartup startup = new MockStartup();
            startup.AddServices(new RemotePlusServer.Core.ServiceCollection());
            UserCredentials cred = new UserCredentials(username, password);
            var account = ServerManager.AccountManager.AttemptLogin(cred);
            Assert.NotNull(account);
        }
    }
}