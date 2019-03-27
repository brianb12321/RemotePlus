using RemotePlusLibrary.Discovery;
using RemotePlusLibrary.SubSystem.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProxyServer.Scripting.Batch
{
    public class TaskGroup
    {
        public static TaskGroup withAllServers()
        {
            var tg = new TaskGroup();
            foreach(var server in ProxyManager.ProxyService.RemoteInterface.ConnectedServers)
            {
                tg.addServer(ProxyManager.ProxyService.RemoteInterface.ConnectedServers.IndexOf(server));
            }
            return tg;
        }
        protected List<JobTask> tasks = new List<JobTask>();
        protected JobExecutionMode executionMode = JobExecutionMode.Single;
        protected JobExecutionMode taskExecutionMode = JobExecutionMode.Single;
        protected List<SessionClient<IRemoteWithProxy>> servers = new List<SessionClient<IRemoteWithProxy>>();
        public virtual void addServer(int serverID)
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
        public virtual void addTask(JobTask task)
        {
            tasks.Add(task);
        }
        public virtual JobTask addTask(Action<SessionClient<IRemoteWithProxy>> task)
        {
            JobTask t = new JobTask(task);
            tasks.Add(t);
            return t;
        }
        public virtual JobTask addCommandTask(string command)
        {
            return addTask((s) =>
            {
                s.ClientCallback.RunServerCommand(command, CommandExecutionMode.Script);
            });
        }
        public virtual JobTask addScriptTask(string script)
        {
            return addTask((s) =>
            {
                s.ClientCallback.ExecuteScript(script);
            });
        }
        public virtual JobTask replaceTask(int taskPosition, Action<SessionClient<IRemoteWithProxy>> task)
        {
            JobTask t = new JobTask(task);
            tasks[taskPosition] = t;
            return t;
        }
        public virtual void replaceTask(int taskPosition, JobTask task)
        {
            tasks[taskPosition] = task;
        }
        public virtual void removeTask(int taskPosition)
        {
            tasks.RemoveAt(taskPosition);
        }
        public virtual void removeTask(JobTask task)
        {
            tasks.Remove(task);
        }
        public virtual void run()
        {
            if (executionMode == JobExecutionMode.Parallel)
            {
                if (taskExecutionMode == JobExecutionMode.Parallel)
                {
                    servers?.AsParallel().ForAll(s => tasks.AsParallel().ForAll(t => t.run(s)));
                }
                else
                {
                    servers?.AsParallel().ForAll(s => tasks.ForEach(t => t.run(s)));
                }
            }
            else
            {
                if (taskExecutionMode == JobExecutionMode.Parallel)
                {
                    servers?.ForEach(s => tasks.AsParallel().ForAll(t => t.run(s)));
                }
                else
                {
                    servers?.ForEach(s => tasks.ForEach(t => t.run(s)));
                }
            }
        }
        public virtual void setTaskExecutionMode(JobExecutionMode mode)
        {
            taskExecutionMode = mode;
        }
        public virtual void setExecutionMode(JobExecutionMode mode)
        {
            executionMode = mode;
        }
        public virtual void clearAllTasks()
        {
            tasks = new List<JobTask>();
        }
    }
}