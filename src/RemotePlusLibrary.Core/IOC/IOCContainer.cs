using Ninject;

namespace RemotePlusLibrary.Core.IOC
{
    public static class IOCContainer
    {
        public static IKernel Provider { get; set; } = new StandardKernel();
        public static TService GetService<TService>()
        {
            return Provider.Get<TService>();
        }
        public static TService GetService<TService>(string name)
        {
            return Provider.Get<TService>(name);
        }
    }
}