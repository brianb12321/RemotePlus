using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusServer
{
    public class EndpointWrapper<E> where E : new()
    {
        public E EndpointRemote { get; private set; }
        public EndpointWrapper(E singleTon)
        {
            EndpointRemote = singleTon;
        }
    }
}
