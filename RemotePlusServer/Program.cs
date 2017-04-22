using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using RemotePlusLibrary;
using System.Threading;
using System.Speech.Synthesis;
using CommandLine;
using System.ServiceModel.Description;
using RemotePlusLibrary.Extension;
using RemotePlusLibrary.Extension.Helper;
using System.IO;
using Logging;

namespace RemotePlusServer
{
    public class ServerManager
    {
        public static CMDLogging Logger { get; } = new CMDLogging();
        public static ServiceHost host { get; private set; } = null;
        public static RemoteImpl Remote { get; } = new RemoteImpl();

        public static ServerSettings DefaultSettings { get; set; } = new ServerSettings();
        static void Main(string[] args)
        {
            Logger.DefaultFrom = "Server Host";
            ScanForServerSettingsFile();
            RunInServerMode();
        }
        static void RunInServerMode()
        {
            string url = $"net.tcp://127.0.0.1:{DefaultSettings.PortNumber}/Remote";
            host = new ServiceHost(Remote);
            NetTcpBinding tcp = new NetTcpBinding();
            Logger.AddOutput("Loading extensions...", Logging.OutputLevel.Info);
            foreach (string files in Directory.GetFiles("extensions"))
            {
                if (Path.GetExtension(files) == ".dll")
                {
                    Logger.AddOutput($"Found extension file ({Path.GetFileName(files)})", Logging.OutputLevel.Info);
                    foreach (ServerExtension ext in ExtensionLoader.Load(files))
                    {
                        Remote.AddExtension(ext);
                        Logger.AddOutput($"Extension {ext.GeneralDetails.Name} loaded.", Logging.OutputLevel.Info);
                    }
                }
            }
            tcp.MaxReceivedMessageSize = 2147483647;
            tcp.MaxBufferSize = 2147483647;
            tcp.ReaderQuotas.MaxArrayLength = 2147483647;
            tcp.ReaderQuotas.MaxDepth = 2147483647;
            tcp.ReaderQuotas.MaxStringContentLength = 2147483647;
            tcp.ReaderQuotas.MaxBytesPerRead = 2147483647;
            tcp.ReaderQuotas.MaxNameTableCharCount = 2147483647;
            host.AddServiceEndpoint(typeof(IRemote), tcp, url);
            host.Open();
            Logger.AddOutput("Host ready. Connect to configure server.", Logging.OutputLevel.Info);
            Console.ReadLine();
            host.Close();
        }
        static void ScanForServerSettingsFile()
        {
            if (!File.Exists("GlobalServerSettings.config"))
            {
                Logger.AddOutput("Creating server settings file.", OutputLevel.Info);
                DefaultSettings.Save();
            }
            else
            {
                Logger.AddOutput("Loading server settings file.", OutputLevel.Info);
                DefaultSettings.Load();
            }
        }

        private static void Default_SettingsLoaded(object sender, System.Configuration.SettingsLoadedEventArgs e)
        {
            Logger.AddOutput("Settings loaded.", OutputLevel.Info);
        }

        public static List<Logging.LogItem> Execute(string c)
        {
            List<Logging.LogItem> l = new List<Logging.LogItem>();
            try
            {
                List<string> obj = new List<string>();
                string[] ca = c.Split();
                if(ca[0] == "ex")
                {
                    for(int i = 2; i < ca.Length; i++)
                    {
                        obj.Add(ca[i]);
                    }
                    Remote.RunExtension(ca[1], obj.ToArray());
                    l.Add(Logger.AddOutput("Extension executed.", OutputLevel.Info));
                }
                return l;
            }
            catch (Exception ex)
            {
                l.Add(Logger.AddOutput("Error whie executing command: " + ex.Message, OutputLevel.Error));
                return l;
            }
        }
        private static void Help()
        {
            
        }

        static void Close()
        {
            host.Close();
            Environment.Exit(0);
        }
    }
}