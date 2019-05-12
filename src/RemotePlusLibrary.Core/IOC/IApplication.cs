using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Core.IOC
{
    public interface IApplication
    {
        Guid EnvironmentGuid { get; set; }
        NetworkSide ExecutingSide { get; }
        EnvironmentState State { get; }
        Task Start(string[] args);
        void Close();
    }
}