using RemotePlusLibrary.Discovery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyServer.Scripting.Batch
{
    public class BatchJob
    {
        private List<SessionClient<IRemoteWithProxy>> servers = new List<SessionClient<IRemoteWithProxy>>();
        private List<BatchTask> tasks = new List<BatchTask>();
        private JobExecutionMode executionMode = JobExecutionMode.Single;
        public void addTask(BatchTask task)
        {
            tasks.Add(task);
        }
        public BatchTask addTask(Action<SessionClient<IRemoteWithProxy>> task)
        {
            BatchTask t = new BatchTask(task);
            tasks.Add(t);
            return t;
        }
        public BatchTask replaceTask(int taskPosition, Action<SessionClient<IRemoteWithProxy>> task)
        {
            BatchTask t = new BatchTask(task);
            tasks[taskPosition] = t;
            return t;
        }
        public void replaceTask(int taskPosition, BatchTask task)
        {
            tasks[taskPosition] = task;
        }
        public void removeTask(int taskPosition)
        {
            tasks.RemoveAt(taskPosition);
        }
        public void addServer(int serverID)
        {
            try
            {
                servers.Add(ProxyManager.ProxyService.RemoteInterface.ConnectedServers[serverID]);
            }
            catch (KeyNotFoundException)
            {
                throw new KeyNotFoundException("The client does not exist.");
            }
        }
        public BatchTask addCommandTask(string command)
        {
            return addTask((s) =>
            {
                s.ClientCallback.RunServerCommand(command, RemotePlusLibrary.Extension.CommandSystem.CommandExecutionMode.Script);
            });
        }
        public BatchTask addScriptTask(string script)
        {
            return addTask((s) =>
            {
                s.ClientCallback.ExecuteScript(script);
            });
        }
        public void setExecutionMode(JobExecutionMode mode)
        {
            executionMode = mode;
        }
        public void run()
        {
            if (executionMode == JobExecutionMode.Parallel)
            {
                servers.AsParallel().ForAll(s => tasks.ForEach(a => a.run(s)));
            }
            else
            {
                servers.ForEach(s => tasks.ForEach((a) => a.run(s)));
            }
        }
    }
}