using RemotePlusLibrary.Extension;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

namespace RemotePlusClient
{
    public static class ExtensionManager
    {
        public static Dictionary<string, IClientExtension> Load(string FileName)
        {
            Dictionary<string, IClientExtension> l = new Dictionary<string, IClientExtension>();
            Assembly a = Assembly.LoadFrom(FileName);
            foreach(Type t in a.GetTypes())
            {
                if(t.IsClass == true && (typeof(IClientExtension).IsAssignableFrom(t)))
                {
                    var f = (IClientExtension)Activator.CreateInstance(t);
                    MainF.ConsoleObj.Logger.AddOutput("Form load: " + f.Details.Name, Logging.OutputLevel.Info);
                    l.Add(f.Details.Name, f);
                }
            }
            return l;
        }
    }
}