using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.Programming
{
    public class RemotePlusProcess
    {
        public Guid ProcessID { get; }
        public IRemotePlusProgram ExecutingProgram { get; }
        public Task<CommandResponse> RunningTask { get; }
        private CancellationTokenSource _cancelSource;
        protected RemotePlusProcess(Guid id, IRemotePlusProgram program, Task<CommandResponse> t, CancellationTokenSource cs)
        {
            ProcessID = id;
            ExecutingProgram = program;
            RunningTask = t;
            _cancelSource = cs;
        }
        public static RemotePlusProcess CreateNew(CommandRequest request, CommandExecutionMode mode, IRemotePlusProgram program)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            RemotePlusProcess rp = new RemotePlusProcess(Guid.NewGuid(), program, new Task<CommandResponse>(() => program.Execute(request, mode), cts.Token), cts);
            return rp;
        }
        public void Cancel()
        {
            _cancelSource.Cancel();
        }
        public void Shutdown()
        {
            _cancelSource.Cancel();
        }
    }
}