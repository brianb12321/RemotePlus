using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.ServiceArchitecture
{
    public interface IServiceManager
    {
        void AddService<TService, TImpl>() where TService : IRemotePlusService<TImpl> where TImpl : new();
        void AddServiceUsingBuilder<TServiceImpl>(Func<IWCFServiceBuilder<TServiceImpl>> builder) where TServiceImpl : class, new();
        IRemotePlusService<TImpl> GetService<TImpl>() where TImpl : new();
    }
}