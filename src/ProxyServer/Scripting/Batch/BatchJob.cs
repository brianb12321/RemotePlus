using RemotePlusLibrary.Discovery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyServer.Scripting.Batch
{
    public class BatchJob : TaskGroup
    {
        private List<TaskGroup> taskGroups = new List<TaskGroup>();
        public BatchJob()
        {
            taskGroups.Add(new TaskGroup());
        }
        public override void addServer(int serverID)
        {
            taskGroups[0].addServer(serverID);
        }
        public override JobTask addTask(Action<SessionClient<IRemoteWithProxy>> task)
        {
            return taskGroups[0].addTask(task);
        }
        public override JobTask addCommandTask(string command)
        {
            return taskGroups[0].addCommandTask(command);
        }
        public override JobTask addScriptTask(string script)
        {
            return taskGroups[0].addScriptTask(script);
        }
        public override void addTask(JobTask task)
        {
            taskGroups[0].addTask(task);
        }
        public void addTaskGroup(TaskGroup tg)
        {
            taskGroups.Add(tg);
        }
        public void removeTaskGroup(int taskGroupPosition)
        {
            taskGroups.RemoveAt(taskGroupPosition);
        }
        public void removeTaskGroup(TaskGroup tg)
        {
            taskGroups.Remove(tg);
        }
        public override void removeTask(int taskPosition)
        {
            taskGroups[0].removeTask(taskPosition);
        }
        public override void removeTask(JobTask task)
        {
            taskGroups[0].removeTask(task);
        }
        public override JobTask replaceTask(int taskPosition, Action<SessionClient<IRemoteWithProxy>> task)
        {
            return taskGroups[0].replaceTask(taskPosition, task);
        }
        public override void replaceTask(int taskPosition, JobTask task)
        {
            taskGroups[0].replaceTask(taskPosition, task);
        }
        public override void setTaskExecutionMode(JobExecutionMode mode)
        {
            taskGroups[0].setTaskExecutionMode(mode);
        }
        public override void setExecutionMode(JobExecutionMode mode)
        {
            taskGroups[0].setExecutionMode(mode);
        }
        public override void run()
        {
            foreach(TaskGroup tg in taskGroups)
            {
                tg.run();
            }
        }
        public override void clearAllTasks()
        {
            taskGroups = new List<TaskGroup>();
        }
    }
}