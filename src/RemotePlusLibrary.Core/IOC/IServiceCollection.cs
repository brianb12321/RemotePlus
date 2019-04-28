using System.Runtime.Remoting.Services;

namespace RemotePlusLibrary.Core.IOC
{
    /// <summary>
    /// Represents all the services in the DI container.
    /// </summary>
    public interface IServiceCollection
    {
        IServiceCollection AddSingleton<TService>(TService service);
        IServiceCollection AddSingleton<TService>();
        IServiceCollection AddSingleton<TService, TServiceImpl>();
        IServiceCollection AddSingletonNamed<TService, TServiceImpl>(string name);
        IServiceCollection AddSingletonNamed<TService>(string name, TService service);
        IServiceCollection AddTransient<TService, TImplementation>();
        IServiceCollection AddTransient<TService>();
        TService GetService<TService>();
        TService GetServiceNamed<TService>(string name);
        TService[] GetAllServices<TService>();
    }
}