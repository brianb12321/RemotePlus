using Ninject;

namespace RemotePlusLibrary.Core.IOC
{
    public static class IOCContainer
    {
        public static IServiceCollection Provider { get; set; } = new ServiceCollection();
        public static TService GetService<TService>()
        {
            return Provider.GetService<TService>();
        }
        public static TService GetService<TService>(string name)
        {
            return Provider.GetServiceNamed<TService>(name);
        }
    }
}