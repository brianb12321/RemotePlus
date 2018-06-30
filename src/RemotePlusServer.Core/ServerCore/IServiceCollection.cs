using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusServer.Core.ServerCore
{
    public interface IServiceCollection
    {
        IServiceCollection AddSingleton<TService>(TService service);
    }
}
