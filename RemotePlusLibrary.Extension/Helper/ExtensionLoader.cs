using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.Helper
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
                    st.Init();
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
