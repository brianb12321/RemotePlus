using System;
using System.Collections.Generic;
using System.ServiceModel;
using RemotePlusLibrary;
using RemotePlusLibrary.Extension;
using System.IO;
using Logging;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.Core;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using RemotePlusLibrary.Core.EmailService;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using RemotePlusServer.ExtensionSystem;
using RemotePlusLibrary.Scripting;
using RemotePlusServer.Proxies;
using RemotePlusLibrary.FileTransfer.Service;
using System.ServiceModel.Description;

namespace RemotePlusServer
{
    /// <summary>
    /// Provides global access to the server instance.
    /// </summary>
    public static partial class ServerManager
    {
        /// <summary>
        /// The global logger for the server.
        /// </summary>
        public static CMDLogging Logger { get; } = new CMDLogging();
        /// <summary>
        /// The global service that is running on the server. Use this property to access server specific functions.
        /// </summary>
        public static RemotePlusService<RemoteImpl> DefaultService { get; private set; }
        /// <summary>
        /// The main server configuration. Provides settings for the main server.
        /// </summary>
        public static ServerSettings DefaultSettings { get; set; } = new ServerSettings();
        /// <summary>
        /// The main email configuration. Provides settings for the default SMTP settings and sets the behavior of the SMTP client.
        /// </summary>
        public static EmailSettings DefaultEmailSettings { get; set; } = new EmailSettings();
        /// <summary>
        /// The global container that all house the libraries that are loaded into the system.
        /// </summary>
        public static ServerExtensionLibraryCollection DefaultCollection { get; } = new ServerExtensionLibraryCollection();
        /// <summary>
        /// The remote implementation of the file service.
        /// </summary>
        private static Stopwatch sw;
        public static ScriptBuilder ScriptBuilder { get; } = new ScriptBuilder();
        public static RemotePlusService<FileTransferService> FileTransferService { get; private set; }
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
//#if !DEBUG
//                AppDomain.CurrentDomain.FirstChanceException += (sender, e) => Logger.AddOutput($"Error occured during server execution: {e.Exception.Message}", OutputLevel.Error);
//#else
//                AppDomain.CurrentDomain.FirstChanceException += (sender, e) => Logger.AddOutput($"Error occured during server execution: {e.Exception.ToString()}", OutputLevel.Error);
//#endif
                var a = Assembly.GetExecutingAssembly().GetName();
                Console.WriteLine($"Welcome to {a.Name}, version: {a.Version.ToString()}\n\n");
                Logger.DefaultFrom = "Server Host";
                Logger.AddOutput("Starting stop watch.", OutputLevel.Debug);
                Logger.AddOutput(new LogItem(OutputLevel.Info, "NOTE: Tracing may be enabled on the server.", "Server Host") { Color = ConsoleColor.Cyan });
                sw = new Stopwatch();
                sw.Start();
                InitalizeKnownTypes();
                ScanForServerSettingsFile();
                ScanForEmailSettingsFile();
                CreateServer();
                InitializeVariables();
                InitializeCommands();
                InitializeGlobals();
                ScriptBuilder.InitializeEngine();
                if (CheckPrerequisites())
                {
                    bool autoStart = false;
                    if(args.Length == 1)
                    {
                        autoStart = true;
                    }
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new ServerControls(autoStart));
                }
            }
            catch(Exception ex)
            {
                if(Debugger.IsAttached)
                {
                    throw;
                }
#if !COGNITO
                Logger.AddOutput("Internal server error: " + ex.Message, OutputLevel.Error);
                Console.Write("Press any key to exit.");
                Console.ReadKey();
                SaveLog();
#else
                MessageBox.Show("Internal server error: " + ex.Message);
                SaveLog();
#endif

            }
        }
        internal static void InitializeGlobals()
        {
            try
            {
                ScriptBuilder.AddScriptObject("serverInstance", new LuaServerInstance(), "Provides access to the global server instance.", ScriptGlobalType.Variable);
                ScriptBuilder.AddScriptObject("executeServerCommand", new Func<string, CommandPipeline>((command => ServerManager.DefaultService.Remote.RunServerCommand(command, RemotePlusLibrary.Extension.CommandSystem.CommandExecutionMode.Script))), "Executes a command to the server.", ScriptGlobalType.Function);
                ScriptBuilder.AddScriptObject("speak", new Action<string, int, int>(StaticRemoteFunctions.speak), "Makes the server speak.", ScriptGlobalType.Function);
                ScriptBuilder.AddScriptObject("beep", new Action<int, int>(StaticRemoteFunctions.beep), "Makes the server beep.", ScriptGlobalType.Function);
                ScriptBuilder.AddScriptObject("functionExists", new Func<string, bool>((name) => ScriptBuilder.FunctionExists(name)), "Returns true if the function exists in the server.", ScriptGlobalType.Function);
                ScriptBuilder.AddScriptObject("createRequestBuilder", new Func<string, string, Dictionary<string, string>, RequestBuilder>(ClientInstance.createRequestBuilder), "Generates a request builder to be used to generate a request.", ScriptGlobalType.Function);
                ScriptBuilder.AddScriptObject("clientPrint", new Action<string>((text => DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(text))), "Prints the text to the client-console", ScriptGlobalType.Function);
            }
            catch (ArgumentException)
            {

            }
        }
        private static void ScanForEmailSettingsFile()
        {
            if (!File.Exists(EmailSettings.EMAIL_CONFIG_FILE))
            {
                Logger.AddOutput("The email settings file does not exist. Creating server settings file.", OutputLevel.Warning);
                DefaultEmailSettings.Save();
            }
            else
            {
                Logger.AddOutput("Loading email settings file.", OutputLevel.Info);
                try
                {
                    DefaultEmailSettings.Load();
                }
                catch (Exception ex)
                {
#if DEBUG
                    Logger.AddOutput("Unable to load email settings. " + ex.ToString(), OutputLevel.Error);
#else
                    Logger.AddOutput("Unable to load email settings. " + ex.Message, OutputLevel.Error);
#endif
                }
            }
        }

        private static void CreateServer()
        {
            DefaultService = RemotePlusService<RemoteImpl>.Create(typeof(IRemote), new RemoteImpl(), DefaultSettings.PortNumber,"Remote", (m, o) => Logger.AddOutput(m, o), null);
            SetupFileTransferService();
            LoadExtensionLibraries();
            OpenMex();
            DefaultService.Remote.Setup();
            Logger.AddOutput("Attaching server events.", OutputLevel.Debug);
            DefaultService.HostClosed += Host_Closed;
            DefaultService.HostClosing += Host_Closing;
            DefaultService.HostFaulted += Host_Faulted;
            DefaultService.HostOpened += Host_Opened;
            DefaultService.HostOpening += Host_Opening;
            DefaultService.HostUnknownMessageReceived += Host_UnknownMessageReceived;
        }

        private static void OpenMex()
        {
            if(DefaultSettings.EnableMetadataExchange)
            {
                Logger.AddOutput(new LogItem(OutputLevel.Info, "NOTE: Metadata exchange is enabled on the server.", "Server Host" ) { Color = ConsoleColor.Cyan });
                System.ServiceModel.Channels.Binding mexBinding = MetadataExchangeBindings.CreateMexHttpBinding();
                ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
                smb.HttpGetEnabled = true;
                smb.HttpGetUrl = new Uri("http://0.0.0.0:9001/Mex");
                ServiceMetadataBehavior smb2 = new ServiceMetadataBehavior();
                smb.HttpGetEnabled = true;
                smb.HttpGetUrl = new Uri("http://0.0.0.0:9001/Mex2");
                DefaultService.Host.Description.Behaviors.Add(smb);
                FileTransferService.Host.Description.Behaviors.Add(smb2);
                DefaultService.Host.AddServiceEndpoint(typeof(IMetadataExchange), mexBinding, "http://0.0.0.0:9001/Mex");
                FileTransferService.Host.AddServiceEndpoint(typeof(IMetadataExchange), mexBinding, "http://0.0.0.0:9001/Mex2");
            }
        }

        private static void SetupFileTransferService()
        {
            Logger.AddOutput("Adding file transfer service.", OutputLevel.Info);
            var binding = _ConnectionFactory.BuildBinding();
            binding.TransferMode = TransferMode.Streamed;
            FileTransferService = RemotePlusService<FileTransferService>.CreateNotSingle(typeof(IFileTransferContract), DefaultSettings.PortNumber, binding, "FileTransfer", null);
            FileTransferService.HostClosed += Host_Closed;
            FileTransferService.HostClosing += Host_Closing;
            FileTransferService.HostFaulted += Host_Faulted;
            FileTransferService.HostOpened += Host_Opened;
            FileTransferService.HostOpening += Host_Opening;
            FileTransferService.HostUnknownMessageReceived += Host_UnknownMessageReceived;
        }

        private static void SaveLog()
        {
            try
            {
                if (DefaultSettings.LoggingSettings.LogOnShutdown)
                {
                    Logger.AddOutput("Saving log and closing.", OutputLevel.Info);
                    Logger.SaveLog($"{DefaultSettings.LoggingSettings.LogFolder}\\{DateTime.Now.ToShortDateString().Replace('/', DefaultSettings.LoggingSettings.DateDelimiter)} {DateTime.Now.ToShortTimeString().Replace(':', DefaultSettings.LoggingSettings.TimeDelimiter)}.txt");
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
            DefaultService.Commands.Add("version", version);
            DefaultService.Commands.Add("encrypt", svm_encyptFile);
            DefaultService.Commands.Add("decrypt", svm_decryptFile);
            DefaultService.Commands.Add("beep", svm_beep);
            DefaultService.Commands.Add("speak", svm_speak);
            DefaultService.Commands.Add("showMessageBox", svm_showMessageBox);
            DefaultService.Commands.Add("path", path);
            DefaultService.Commands.Add("cd", cd);
            DefaultService.Commands.Add("echo", echo);
            DefaultService.Commands.Add("load-extensionLibrary", loadExtensionLibrary);
            DefaultService.Commands.Add("cp", cp);
            DefaultService.Commands.Add("deleteFile", deleteFile);
            DefaultService.Commands.Add("echoFile", echoFile);
            DefaultService.Commands.Add("ls", ls);
            DefaultService.Commands.Add("genMan", genMan);
            DefaultService.Commands.Add("scp", scp);
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
            List<string> excludedFiles = new List<string>();
            Logger.AddOutput("Loading extensions...", Logging.OutputLevel.Info);
            if (Directory.Exists("extensions"))
            {
                if (File.Exists("extensions\\excludes.txt"))
                {
                    Logger.AddOutput("Found an excludes.txt file. Reading file...", OutputLevel.Info);
                    foreach(string excludedFile in File.ReadLines("extensions\\excludes.txt"))
                    {
                        Logger.AddOutput($"{excludedFile} is excluded from the extension search.", OutputLevel.Info);
                        excludedFiles.Add("extensions\\" + excludedFile);
                    }
                    Logger.AddOutput("Finished reading extension exclusion file.", OutputLevel.Info);
                }
                ServerInitEnvironment env = new ServerInitEnvironment(false);
                foreach (string files in Directory.GetFiles("extensions"))
                {
                    if (Path.GetExtension(files) == ".dll" && !excludedFiles.Contains(files))
                    {
                        try
                        {
                            Logger.AddOutput($"Found extension file ({Path.GetFileName(files)})", Logging.OutputLevel.Info);
                            env.PreviousError = Logger.errorcount > 0 ? true : false;
                            var lib = ServerExtensionLibrary.LoadServerLibrary(files, (m, o) => Logger.AddOutput(m, o), env);
                            DefaultCollection.Libraries.Add(lib.Name, lib);
                        }
                        catch (Exception ex)
                        {
                            Logger.AddOutput($"Could not load \"{files}\" because of a load error or initialization error. Error: {ex.Message}", OutputLevel.Error);
                        }
                        env.InitPosition++;
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
            FileTransferService.Start();
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
        public static CommandResponse Execute(CommandRequest c, CommandExecutionMode commandMode, CommandPipeline pipe)
        {
            bool throwFlag = false;
            StatusCodeDeliveryMethod scdm = StatusCodeDeliveryMethod.DoNotDeliver;
            try
            {
                ServerManager.Logger.AddOutput($"Executing server command {c.Arguments[0]}", OutputLevel.Info);
                try
                {
                    var command = DefaultService.Commands[c.Arguments[0].Value];
                    var ba = RemotePlusConsole.GetCommandBehavior(command);
                    if (ba != null)
                    {
                        if(ba.TopChainCommand && pipe.Count > 0)
                        {
                            Logger.AddOutput($"This is a top-level command.", OutputLevel.Error);
                            DefaultService.Remote.Client.ClientCallback.TellMessage($"This is a top-level command.", OutputLevel.Error);
                            return new CommandResponse((int)CommandStatus.AccessDenied);
                        }
                        if (commandMode != ba.ExecutionType)
                        {
                            Logger.AddOutput($"The command requires you to be in {ba.ExecutionType} mode.", OutputLevel.Error);
                            DefaultService.Remote.Client.ClientCallback.TellMessage($"The command requires you to be in {ba.ExecutionType} mode.", OutputLevel.Error);
                            return new CommandResponse((int)CommandStatus.AccessDenied);
                        }
                        if(ba.SupportClients != ClientSupportedTypes.Both && ((DefaultService.Remote.Client.ClientType == ClientType.GUI && ba.SupportClients != ClientSupportedTypes.GUI) || (DefaultService.Remote.Client.ClientType == ClientType.CommandLine && ba.SupportClients != ClientSupportedTypes.CommandLine)))
                        {
                            if(string.IsNullOrEmpty(ba.ClientRejectionMessage))
                            {
                                Logger.AddOutput($"Your client must be a {ba.SupportClients.ToString()} client.", OutputLevel.Error);
                                DefaultService.Remote.Client.ClientCallback.TellMessage($"Your client must be a {ba.SupportClients.ToString()} client.", OutputLevel.Error);
                                return new CommandResponse((int)CommandStatus.UnsupportedClient);
                            }
                            else
                            {
                                Logger.AddOutput(ba.ClientRejectionMessage, OutputLevel.Error);
                                DefaultService.Remote.Client.ClientCallback.TellMessage(ba.ClientRejectionMessage, OutputLevel.Error);
                                return new CommandResponse((int)CommandStatus.UnsupportedClient);
                            }
                        }
                        if (ba.DoNotCatchExceptions)
                        {
                            throwFlag = true;
                        }
                        if (ba.StatusCodeDeliveryMethod != StatusCodeDeliveryMethod.DoNotDeliver)
                        {
                            scdm = ba.StatusCodeDeliveryMethod;
                        }
                    }
                    Logger.AddOutput("Found command, and executing.", OutputLevel.Debug);
                    var sc = command(c, pipe);
                    if (scdm == StatusCodeDeliveryMethod.TellMessage)
                    {
                        DefaultService.Remote.Client.ClientCallback.TellMessage($"Command {c.Arguments[0]} finished with status code {sc.ToString()}", OutputLevel.Info);
                    }
                    else if (scdm == StatusCodeDeliveryMethod.TellMessageToServerConsole)
                    {
                        DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Info, $"Command {c.Arguments[0]} finished with status code {sc.ToString()}"));
                    }
                    return sc;
                }
                catch (KeyNotFoundException)
                {
                    Logger.AddOutput("Failed to find the command.", OutputLevel.Debug);
                    DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Error, "Unknown command. Please type {help} for a list of commands", "Server Host"));
                    return new CommandResponse((int)CommandStatus.Fail);
                }
            }
            catch (Exception ex)
            {
                if (throwFlag)
                {
                    throw;
                }
                else
                {
                    ServerManager.Logger.AddOutput("command failed: " + ex.Message, OutputLevel.Info);
                    DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Error, "Error whie executing command: " + ex.Message, "Server Host"));
                    return new CommandResponse((int)CommandStatus.Fail);
                }
            }
        }

        public static void Close()
        {
            SaveLog();
            DefaultService.Close();
            FileTransferService.Close();
            Environment.Exit(0);
        }
    }
}