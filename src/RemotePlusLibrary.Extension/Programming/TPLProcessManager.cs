using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;

namespace RemotePlusLibrary.Extension.Programming
{
    public class TPLProcessManager : IProcessManager
    {
        Dictionary<Guid, RemotePlusProcess> _processes;
        public RemotePlusProcess GetProcess(Guid id)
        {
            return _processes[id];
        }

        public void Kill(Guid processID)
        {
            _processes[processID].Cancel();
        }

        public void Shutdown(Guid processID)
        {
            _processes[processID].Cancel();
        }

        public Task<CommandResponse> Start(CommandRequest args, CommandExecutionMode mode, IRemotePlusProgram program)
        {
            RemotePlusProcess process = RemotePlusProcess.CreateNew(args, mode, program);
            _processes.Add(process.ProcessID, process);
            process.RunningTask.Start();
            return process.RunningTask;
        }
    }
}