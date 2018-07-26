using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Core.IOC
{
    public interface IServiceCollection
    {
        IServiceCollection AddSingleton<TService>(TService service);
        IServiceCollection AddSingleton<TService>();
        IServiceCollection AddSingleton<TService, TServiceImpl>();
        IServiceCollection AddSingletonNamed<TService, TServiceImpl>(string name);
        IServiceCollection AddTransient<TService, TImplementation>();
        IServiceCollection AddTransient<TService>();
    }
}