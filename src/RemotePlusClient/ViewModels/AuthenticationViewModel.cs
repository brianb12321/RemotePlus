using RemotePlusLibrary.Security.AccountSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusClient.ViewModels
{
    public class AuthenticationViewModel : BaseViewModel
    {
        public UserCredentials Credentails { get; set; }
        public AuthenticationViewModel(string fn) : base(fn)
        {
        }
    }
}
