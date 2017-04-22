using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary
{
    public class Client
    {
        public string FriendlyName { get; internal set; }
        public IRemoteClient ClientCallback { get; internal set; }
        internal Client()
        {

        }
    }
}