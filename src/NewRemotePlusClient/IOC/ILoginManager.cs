using RemotePlusLibrary.Security.AccountSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewRemotePlusClient.IOC
{
    public interface ILoginManager
    {
        UserCredentials ShowLogin();
    }
}