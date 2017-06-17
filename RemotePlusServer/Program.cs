using System;
using System.Collections.Generic;
using System.ServiceModel;
using RemotePlusLibrary;
using RemotePlusLibrary.Extension;
using System.IO;
using Logging;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.Extension.WatcherSystem;
using RemotePlusLibrary.Core;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace RemotePlusServer
{
    public static partial class ServerManager
    {
        public static Dictionary<string, WatcherBase> Watchers { get; private set; }
        public static CMDLogging Logger { get; } = new CMDLogging();
        public static RemotePlusService<RemoteImpl> DefaultService { get; private set; }
        public static ServerSettings DefaultSettings { get; set; } = new ServerSettings();
        public static ServerExtensionLibraryCollection DefaultCollection { get; } = new ServerExtensionLibraryCollection();
        private static Stopwatch sw;
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                var a = Assembly.GetExecutingAssembly().GetName();
                Console.WriteLine($"Welcome to {a.Name}, version: {a.Version.ToString()}\n\n");
                Logger.DefaultFrom = "Server Host";
                Logger.AddOutput("Starting stop watch.", OutputLevel.Debug);
                sw = new Stopwatch();
                sw.Start();
                InitalizeKnownTypes();
                ScanForServerSettingsFile();
                InitializeWatchers();
                CreateServer();
                InitializeVariables();
                InitializeCommands();
                if (CheckPrerequisites())
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new ServerControls());
                }
            }
            catch(Exception ex)
            {
                Logger.AddOutput("Internal server error: " + ex.Message, OutputLevel.Error);
                Console.Write("Press any key to exit.");
                Console.ReadKey();
                SaveLog();
            }
        }

        private static void CreateServer()
        {
            DefaultService = RemotePlusService<RemoteImpl>.Create(new RemoteImpl(), DefaultSettings.PortNumber, (m, o) => Logger.AddOutput(m, o), null);
            LoadExtensionLibraries();
            DefaultService.Remote.Setup();
            Logger.AddOutput("Attaching server events.", OutputLevel.Debug);
            DefaultService.HostClosed += Host_Closed;
            DefaultService.HostClosing += Host_Closing;
            DefaultService.HostFaulted += Host_Faulted;
            DefaultService.HostOpened += Host_Opened;
            DefaultService.HostOpening += Host_Opening;
            DefaultService.HostUnknownMessageReceived += Host_UnknownMessageReceived;
        }

        private static void SaveLog()
        {
            try
            {
                if (DefaultSettings.LogOnShutdown)
                {
                    Logger.AddOutput("Saving log and closing.", OutputLevel.Info);
                    Logger.SaveLog($"ServerLogs\\{DateTime.Now.ToShortDateString().Replace('/', DefaultSettings.DateDelimiter)} {DateTime.Now.ToShortTimeString().Replace(':', DefaultSettings.TimeDelimiter)}.txt");
                }
            }
            catch (Exception ex)
            {
                Logger.AddOutput($"Unable to save log file: {ex.Message}", OutputLevel.Error);
            }
        }

        private static void InitalizeKnownTypes()
        {
            Logger.AddOutput("Adding default known types.", OutputLevel.Info);
            DefaultKnownTypeManager.LoadDefaultTypes();
            Logger.AddOutput("Adding UserAccount to known type list.", OutputLevel.Debug);
            DefaultKnownTypeManager.AddType(typeof(UserAccount));
        }

        private static void InitializeVariables()
        {
            if(File.Exists("Variables.xml"))
            {
                Logger.AddOutput("Loading variables.", OutputLevel.Info);
                DefaultService.Variables = VariableManager.Load();
            }
            else
            {
                Logger.AddOutput("There is no variable file. Beginning variable initialization.", OutputLevel.Warning);
                DefaultService.Variables = VariableManager.New();
                DefaultService.Variables.Add("Name", "RemotePlusServer");
                Logger.AddOutput("Saving file.", OutputLevel.Info);
                DefaultService.Variables.Save();
            }
        }

        private static void InitializeCommands()
        {
            Logger.AddOutput("Loading Commands.", OutputLevel.Info);
            DefaultService.Commands.Add("ex", ExCommand);
            DefaultService.Commands.Add("ps", ProcessStartCommand);
            DefaultService.Commands.Add("help", Help);
            DefaultService.Commands.Add("logs", Logs);
            DefaultService.Commands.Add("vars", vars);
            DefaultService.Commands.Add("dateTime", dateTime);
            DefaultService.Commands.Add("processes", processes);
            DefaultService.Commands.Add("watchers", watchers);
            DefaultService.Commands.Add("version", version);
            DefaultService.Commands.Add("encrypt", svm_encyptFile);
            DefaultService.Commands.Add("decrypt", svm_decryptFile);
            DefaultService.Commands.Add("beep", svm_beep);
            DefaultService.Commands.Add("speak", svm_speak);
            DefaultService.Commands.Add("showMessageBox", svm_showMessageBox);
        }

        static bool CheckPrerequisites()
        {
            Logger.AddOutput("Checking prerequisites.", OutputLevel.Info);
            //Check for prerequisites
            ServerPrerequisites.CheckPrivilleges();
            ServerPrerequisites.CheckNetworkInterfaces();
            ServerPrerequisites.CheckSettings();
            Logger.AddOutput("Stopping stop watch.", OutputLevel.Debug);
            sw.Stop();
            // Check results
            if(Logger.errorcount >= 1 && Logger.warningcount == 0)
            {
                Logger.AddOutput($"Unable to start server. ({Logger.errorcount} errors) Elapsed time: {sw.Elapsed.ToString()}", OutputLevel.Error);
                return false;
            }
            else if(Logger.errorcount >= 1 && Logger.warningcount >= 1)
            {
                Logger.AddOutput($"Unable to start server. ({Logger.errorcount} errors, {Logger.warningcount} warnings) Elapsed time: {sw.Elapsed.ToString()}", OutputLevel.Error);
                return false;
            }
            else if(Logger.errorcount == 0 && Logger.warningcount >= 1)
            {
                Logger.AddOutput($"The server can start, but with warnings. ({Logger.warningcount} warnings) Elapsed time: {sw.Elapsed.ToString()}", OutputLevel.Warning);
                return true;
            }
            else
            {
                Logger.AddOutput(new LogItem(OutputLevel.Info, $"Validation passed. Elapsed time: {sw.Elapsed.ToString()}", "Server Host") { Color = ConsoleColor.Green });
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
                        try
                        {
                            Logger.AddOutput($"Found extension file ({Path.GetFileName(files)})", Logging.OutputLevel.Info);
                            var lib = ServerExtensionLibrary.LoadServerLibrary(files, (m, o) => Logger.AddOutput(m, o));
                            DefaultCollection.Libraries.Add(lib.Name, lib);
                        }
                        catch (Exception ex)
                        {
                            Logger.AddOutput($"Could not load \"{files}\" because of a load error or initialization error. Error: {ex.Message}", OutputLevel.Error);
                        }
                    }
                }
            }
            else
            {
                Logger.AddOutput("The extensions folder does not exist.", OutputLevel.Info);
            }
        }
        public static void RunInServerMode()
        {
            DefaultService.Start();
        }

        private static void Host_UnknownMessageReceived(object sender, UnknownMessageReceivedEventArgs e)
        {
            Logger.AddOutput($"The server encountered an unknown message sent by the client. Message: {e.Message.ToString()}", OutputLevel.Error);
        }

        private static void Host_Opening(object sender, EventArgs e)
        {
            Logger.AddOutput("Opening server.", OutputLevel.Info);
        }

        private static void Host_Opened(object sender, EventArgs e)
        {
            Logger.AddOutput($"Host ready. Server is listening on port {DefaultSettings.PortNumber}. Connect to configure server.", Logging.OutputLevel.Info);
        }

        private static void Host_Faulted(object sender, EventArgs e)
        {
            Logger.AddOutput("The server state has been transferred to the faulted state.", OutputLevel.Error);
        }

        private static void Host_Closing(object sender, EventArgs e)
        {
            Logger.AddOutput("Closing the server.", OutputLevel.Info);
        }

        private static void Host_Closed(object sender, EventArgs e)
        {
            Logger.AddOutput("The server is now closed.", OutputLevel.Info);
        }

        static void ScanForServerSettingsFile()
        {
            if (!File.Exists("Configurations\\Server\\GlobalServerSettings.config"))
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
        static void InitializeWatchers()
        {
            Logger.AddOutput("Initializing watchers.", OutputLevel.Info);
            Watchers = new Dictionary<string, WatcherBase>();
        }
        public static int Execute(string c)
        {
            try
            {
                ServerManager.Logger.AddOutput($"Executing server command {c}", OutputLevel.Info);
                bool FoundCommand = false;
                string[] ca = c.Split();
                foreach (KeyValuePair<string, CommandDelegate> k in DefaultService.Commands)
                {
                    if(ca[0] == k.Key)
                    {
                        Logger.AddOutput("Found command, and executing.", OutputLevel.Debug);
                        FoundCommand = true;
                        return k.Value(ca);
                    }
                }
                if (!FoundCommand)
                {
                    Logger.AddOutput("Failed to find the command.", OutputLevel.Debug);
                    DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Error, "Unknown command. Please type {help} for a list of commands", "Server Host"));
                    return (int)CommandStatus.Fail;
                }
                return -2;
            }
            catch (Exception ex)
            {
                ServerManager.Logger.AddOutput("command failed: " + ex.Message, OutputLevel.Info);
                DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Error,"Error whie executing command: " + ex.Message, "Server Host"));
                return (int)CommandStatus.Fail;
            }
        }
        public static void Close()
        {
            SaveLog();
            DefaultService.Close();
            Environment.Exit(0);
        }
    }
}