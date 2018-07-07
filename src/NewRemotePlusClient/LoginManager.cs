using GalaSoft.MvvmLight.Messaging;
using NewRemotePlusClient.IOC;
using RemotePlusLibrary.Security.AccountSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewRemotePlusClient
{
    public class LoginManager : ILoginManager
    {
        public UserCredentials ShowLogin()
        {
            UserCredentials finalCredentails = new UserCredentials();
            Messenger.Default.Register<UserCredentials>(this, (cred) => finalCredentails = cred);
            IOCHelper.UI.Show<Views.LoginView>();
            return finalCredentails;
        }
    }
}
