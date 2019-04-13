using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Core.IOC;

namespace RemotePlusLibrary.Extension
{
    /// <summary>
    /// A class that provides functionality to the system.
    /// </summary>
    public interface IExtensionModule
    {
        void InitializeServices(IServiceCollection kernel);
    }
}