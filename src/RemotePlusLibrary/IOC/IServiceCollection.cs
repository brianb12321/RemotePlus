using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.IOC
{
    public interface IServiceCollection
    {
        IServiceCollection AddSingleton<TService>(TService service);
        IServiceCollection AddTransient<TService, TImplementation>();
        IServiceCollection AddTransient<TService>();
        IServiceCollection AddSingleton<TService>();
    }
}
