using RemotePlusLibrary.Discovery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyServer.Scripting
{
    public class BatchJob
    {
        private List<SessionClient<IRemoteWithProxy>> servers = new List<SessionClient<IRemoteWithProxy>>();
        private List<Action> tasks = new List<Action>();
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
        public void addCommandTask(string command)
        {
            tasks.Add(() =>
            {
                foreach(SessionClient<IRemoteWithProxy> current in servers)
                {
                    current.ClientCallback.RunServerCommand(command, RemotePlusLibrary.Extension.CommandSystem.CommandExecutionMode.Script);
                }
            });
        }
        public void addScriptTask(string script)
        {
            tasks.Add(() =>
            {
                foreach (SessionClient<IRemoteWithProxy> current in servers)
                {
                    current.ClientCallback.ExecuteScript(script);
                }
            });
        }
        public void run()
        {
            tasks.ForEach((a) => a.Invoke());
        }
    }
}