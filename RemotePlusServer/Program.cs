using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using RemotePlusLibrary;
using System.Threading;
using System.Speech.Synthesis;
using System.ServiceModel.Description;
using RemotePlusLibrary.Extension;
using RemotePlusLibrary.Extension.Helper;
using System.IO;
using Logging;

namespace RemotePlusServer
{
    public delegate List<LogItem> CommandDelgate(params string[] args);
    public class ServerManager
    {
        public static CMDLogging Logger { get; } = new CMDLogging();
        public static ServiceHost host { get; private set; } = null;
        public static RemoteImpl Remote { get; } = new RemoteImpl();
        public static Dictionary<string, CommandDelgate> Commands { get; } = new Dictionary<string, CommandDelgate>();

        public static ServerSettings DefaultSettings { get; set; } = new ServerSettings();
        static void Main(string[] args)
        {
            Commands.Add("ex", ExCommand);
            Commands.Add("ps", ProcessStartCommand);
            Commands.Add("help", Help);
            Logger.DefaultFrom = "Server Host";
            ScanForServerSettingsFile();
            LoadExtensionLibraries();
            RunInServerMode();
        }
        static void LoadExtensionLibraries()
        {
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
        }
        static void RunInServerMode()
        {
            string url = $"net.tcp://127.0.0.1:{DefaultSettings.PortNumber}/Remote";
            host = new ServiceHost(Remote);
            NetTcpBinding tcp = new NetTcpBinding();
            Logger.AddOutput("Loading extensions...", Logging.OutputLevel.Info);
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
        [CommandHelp("Starts a new process on the server.")]
        private static List<LogItem> ProcessStartCommand(string[] args)
        {
            List<LogItem> l = new List<LogItem>();
            if (args.Length > 1)
            {
                string a = "";
                for(int i = 2; i < args.Length; i++)
                {
                    a += " " + args[i];
                }
                Remote.RunProgram((string)args[1], a);
                l.Add(Logger.AddOutput("Program start command finished.", OutputLevel.Info));
            }
            else if (args.Length == 1)
            {
                Remote.RunProgram((string)args[1], "");
                l.Add(Logger.AddOutput("Program start command finished.", OutputLevel.Info));
            }
            return l;
        }
        [CommandHelp("Executes a loaded extension on the server.")]
        private static List<LogItem> ExCommand(object[] args)
        {
            List<LogItem> l = new List<LogItem>();
            List<string> obj = new List<string>();
            for (int i = 2; i < args.Length; i++)
            {
                obj.Add((string)args[i]);
            }
            Remote.RunExtension((string)args[1], obj.ToArray());
            l.Add(Logger.AddOutput("Extension executed.", OutputLevel.Info));
            return l;
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
        public static List<Logging.LogItem> Execute(string c)
        {
            List<Logging.LogItem> l = new List<Logging.LogItem>();
            try
            {
                bool FoundCommand = false;
                string[] ca = c.Split();
                foreach (KeyValuePair<string, CommandDelgate> k in Commands)
                {
                    if(ca[0] == k.Key)
                    {
                        FoundCommand = true;
                        l.AddRange(k.Value(ca));
                    }
                }
                if (!FoundCommand)
                {
                    l.Add(Logger.AddOutput("Unknown command. Please type {help} for a list of commands", OutputLevel.Info));
                }
                return l;
            }
            catch (Exception ex)
            {
                l.Add(Logger.AddOutput("Error whie executing command: " + ex.Message, OutputLevel.Error));
                return l;
            }
        }
        [CommandHelp("Displays a list of commands.")]
        private static List<LogItem> Help(string[] arguments)
        {
            List<LogItem> l = new List<Logging.LogItem>();
            string t = "";
            foreach (KeyValuePair<string, CommandDelgate> c in Commands)
            {
                if(c.Value.Method.GetCustomAttributes(false).Length > 0)
                {
                    foreach(object o in c.Value.Method.GetCustomAttributes(false))
                    {
                        if(o is CommandHelpAttribute)
                        {
                            CommandHelpAttribute cha = (CommandHelpAttribute)o;
                            t += $"\n{c.Key}\t{cha.HelpMessage}";
                        }
                    }
                }
                else
                {
                    t += $"\n{c.Key}";
                }
            }
            l.Add(new LogItem(OutputLevel.Info, t, "Server Host"));
            return l;
        }

        static void Close()
        {
            host.Close();
            Environment.Exit(0);
        }
    }
}