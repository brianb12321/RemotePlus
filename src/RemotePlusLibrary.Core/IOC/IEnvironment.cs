using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Core.IOC
{
    public interface IEnvironment
    {
        NetworkSide ExecutingSide { get; }
        EnvironmentState State { get; }
        Task Start(string[] args);
        void Close();
    }
}