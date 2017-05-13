using RemotePlusLibrary.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusServer
{
    public static class ExtensionLoader
    {
        public static ServerExtension[] Load(string FileName)
        {
            List<ServerExtension> l = new List<ServerExtension>();
            Assembly a = Assembly.LoadFrom(FileName);
            foreach(Type t in a.GetTypes())
            {
                if(t.IsClass == true && (typeof(ILibraryStartup).IsAssignableFrom(t)))
                {
                    ILibraryStartup st = (ILibraryStartup)Activator.CreateInstance(t);
                    ServerManager.Logger.AddOutput("Beginning initialization.", Logging.OutputLevel.Info);
                    ServerManager.Logger.DefaultFrom = a.GetName().Name;
                    st.Init();
                    ServerManager.Logger.DefaultFrom = "Server Host";
                    ServerManager.Logger.AddOutput("finished initalization.", Logging.OutputLevel.Info);
                }
                if(t.IsClass == true && (t.IsSubclassOf(typeof(ServerExtension))))
                {
                    l.Add((ServerExtension)Activator.CreateInstance(t));
                }
            }
            return l.ToArray();
        }
    }
}
