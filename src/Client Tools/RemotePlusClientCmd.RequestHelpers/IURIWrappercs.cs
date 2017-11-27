using RemotePlusLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusClientCmd.RequestHelpers
{
    public interface IURIWrappercs<S>
    {
        RequestBuilder Build();
        S BuildAndSend();
    }
}
