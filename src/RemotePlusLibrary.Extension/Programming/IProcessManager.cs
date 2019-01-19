using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.Programming
{
    /// <summary>
    /// Manages all the processes in the RemotePlus system.
    /// </summary>
    public interface IProcessManager
    {
        /// <summary>
        /// Starts a program process.
        /// </summary>
        /// <param name="program"></param>
        Task<CommandResponse> Start(CommandRequest args, CommandExecutionMode mode, IRemotePlusProgram program);
        void Kill(Guid processID);
        void Shutdown(Guid processID);
        RemotePlusProcess GetProcess(Guid id);
    }
}