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
using System.IO;
using Logging;
using System.Security.Principal;
using System.Management;
using System.Net.NetworkInformation;

namespace RemotePlusServer
{
    public delegate void CommandDelgate(params string[] args);
    public static partial class ServerManager
    {
        public static CMDLogging Logger { get; } = new CMDLogging();
        public static ServiceHost host { get; private set; } = null;
        public static RemoteImpl Remote { get; } = new RemoteImpl();
        public static Dictionary<string, CommandDelgate> Commands { get; } = new Dictionary<string, CommandDelgate>();
        public static VariableManager Variables { get; private set; }
        public static ServerSettings DefaultSettings { get; set; } = new ServerSettings();
        [STAThread]
        static void Main(string[] args)
        {
            try
            {                
                Logger.DefaultFrom = "Server Host";
                InitializeCommands();
                ScanForServerSettingsFile();
                InitializeVariables();
                LoadExtensionLibraries();
                if (CheckPrerequisites())
                {
                    RunInServerMode();
                }
            }
            catch(Exception ex)
            {
                Logger.AddOutput("Internal server error: " + ex.Message, OutputLevel.Error);
                Console.Write("Press any key to exit.");
                Console.ReadKey();
            }
        }

        private static void InitializeVariables()
        {
            if(File.Exists("Variables.xml"))
            {
                Logger.AddOutput("Loading variables.", OutputLevel.Info);
                Variables = VariableManager.Load();
            }
            else
            {
                Logger.AddOutput("There is no variable file. Beginning variable initialization.", OutputLevel.Warning);
                Variables = VariableManager.New();
                Variables.Add("Name", "RemotePlusServer");
                Logger.AddOutput("Saving file.", OutputLevel.Info);
                Variables.Save();
            }
        }

        private static void InitializeCommands()
        {
            Logger.AddOutput("Loading commands.", OutputLevel.Info);
            Commands.Add("ex", ExCommand);
            Commands.Add("ps", ProcessStartCommand);
            Commands.Add("help", Help);
            Commands.Add("logs", Logs);
        }

        static bool CheckPrerequisites()
        {
            Logger.AddOutput("Checking prerequisites.", OutputLevel.Info);
            //Check for admin priv.
            WindowsIdentity wi = WindowsIdentity.GetCurrent();
            WindowsPrincipal p = new WindowsPrincipal(wi);
            if(!p.IsInRole(WindowsBuiltInRole.Administrator))
            {
                Logger.AddOutput("The current logged in user is not part of the group \"Administrator\". This may cause certain operations to fail.", OutputLevel.Warning);
            }
            //Check if IP Address is static
            //ManagementObjectSearcher searcher = new ManagementObjectSearcher(@"root\CIMV2", "SELECT * FROM Win32_NetworkAdapterConfiguration");
            //foreach (ManagementObject mo in searcher.Get())
            //{

            //}
            bool foundFlag = false;
            NetworkInterface[] adapt = NetworkInterface.GetAllNetworkInterfaces();
            if (adapt.Length == 0)
            {
                Logger.AddOutput("Unable to find a network adapter. The server requires at least one adapter.", OutputLevel.Error);
            }
            else
            {
                foreach (NetworkInterface nif in adapt)
                {
                    if (nif.GetIPProperties().GetIPv4Properties() != null)
                    {
                        if (!nif.GetIPProperties().GetIPv4Properties().IsDhcpEnabled)
                        {
                            foundFlag = true;
                            break;
                        }
                    }
                }
                if (!foundFlag)
                {
                    Logger.AddOutput("You should have at least one network adapter that has a static IP. This could cause the client fail to connect to the server.", OutputLevel.Info);
                }
            }

            // Check results
            if(Logger.errorcount >= 1 && Logger.warningcount == 0)
            {
                Logger.AddOutput($"Unable to start server. ({Logger.errorcount} errors", OutputLevel.Error);
                return false;
            }
            else if(Logger.errorcount >= 1 && Logger.warningcount >= 1)
            {
                Logger.AddOutput($"Unable to start server. ({Logger.errorcount} errors, {Logger.warningcount} warnings)", OutputLevel.Error);
                return false;
            }
            else if(Logger.errorcount == 0 && Logger.warningcount >= 1)
            {
                Logger.AddOutput($"The server can start, but with warnings. ({Logger.warningcount} warnings)", OutputLevel.Warning);
                return true;
            }
            else
            {
                Logger.AddOutput(new LogItem(OutputLevel.Info, "Validation passed.", "Server Host") { Color = ConsoleColor.Green });
                return true;
            }
        }
        static void LoadExtensionLibraries()
        {
            Logger.AddOutput("Loading extensions...", Logging.OutputLevel.Info);
            if (Directory.Exists("extensions"))
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
            else
            {
                Logger.AddOutput("The extensions folder does not exist.", OutputLevel.Info);
            }
        }
        static void RunInServerMode()
        {
            string url = $"net.tcp://0.0.0.0:{DefaultSettings.PortNumber}/Remote";
            host = new ServiceHost(Remote);
            NetTcpBinding tcp = new NetTcpBinding();
            tcp.MaxReceivedMessageSize = 2147483647;
            tcp.MaxBufferSize = 2147483647;
            tcp.ReaderQuotas.MaxArrayLength = 2147483647;
            tcp.ReaderQuotas.MaxDepth = 2147483647;
            tcp.ReaderQuotas.MaxStringContentLength = 2147483647;
            tcp.ReaderQuotas.MaxBytesPerRead = 2147483647;
            tcp.ReaderQuotas.MaxNameTableCharCount = 2147483647;
            
            host.AddServiceEndpoint(typeof(IRemote), tcp, url);
            host.Open();
            Logger.AddOutput($"Host ready. Server is listening on port {DefaultSettings.PortNumber}. Connect to configure server.", Logging.OutputLevel.Info);
            Console.ReadLine();
            host.Close();
        }
        static void ScanForServerSettingsFile()
        {
            if (!File.Exists("GlobalServerSettings.config"))
            {
                Logger.AddOutput("The server settings file does not exist. Creating server settings file.", OutputLevel.Warning);
                DefaultSettings.Save();
            }
            else
            {
                Logger.AddOutput("Loading server settings file.", OutputLevel.Info);
                try
                {
                    DefaultSettings.Load();
                }
                catch (Exception ex)
                {
#if DEBUG
                    Logger.AddOutput("Unable to load server settings. " + ex.ToString(), OutputLevel.Error);
#else
                    Logger.AddOutput("Unable to load server settings. " + ex.Message, OutputLevel.Error);
#endif
                }
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
                        k.Value(ca);
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
        static void Close()
        {
            host.Close();
            Environment.Exit(0);
        }
    }
}