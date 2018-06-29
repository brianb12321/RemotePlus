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
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using RemotePlusLibrary.Scripting;
using RemotePlusServer.Proxies;
using RemotePlusLibrary.FileTransfer.Service;
using System.ServiceModel.Description;
using RemotePlusLibrary.Security.AccountSystem;
using RemotePlusLibrary.Security.AccountSystem.Policies;
using RemotePlusLibrary.Discovery;
using RemotePlusLibrary.Configuration.ServerSettings;
using RemotePlusLibrary.RequestSystem;
using RemotePlusLibrary.Contracts;
using RemotePlusLibrary.Client;
using Ninject;
using RemotePlusServer.Core;
using RemotePlusServer.Core.ExtensionSystem;

namespace RemotePlusServer
{
    /// <summary>
    /// The class that starts the server.
    /// </summary>
    public static partial class ServerStartup
    {
        static Stopwatch sw = new Stopwatch();
        static Guid ServerGuid = Guid.NewGuid();
        private static RemoteImpl _remote = null;
        [STAThread]
        static void Main(string[] args)
        {
//            try
//            {
//#if !DEBUG
//                AppDomain.CurrentDomain.FirstChanceException += (sender, e) => ServerManager.Logger.AddOutput($"Error occured during server execution: {e.Exception.Message}", OutputLevel.Error);
//#else
//                AppDomain.CurrentDomain.FirstChanceException += (sender, e) => ServerManager.Logger.AddOutput($"Error occured during server execution: {e.Exception.ToString()}", OutputLevel.Error);
//#endif
                var a = Assembly.GetExecutingAssembly().GetName();
                Console.WriteLine($"Welcome to {a.Name}, version: {a.Version.ToString()}\n\n");
                IOCContainer.Setup();
                ServerManager.Logger.DefaultFrom = "Server Host";
                ServerManager.Logger.AddOutput("Starting stop watch.", OutputLevel.Debug);
                ServerManager.Logger.AddOutput(new LogItem(OutputLevel.Info, "NOTE: Tracing may be enabled on the server.", "Server Host") { Color = ConsoleColor.Cyan });
                sw = new Stopwatch();
                sw.Start();
                InitalizeKnownTypes();
                ScanForServerSettingsFile();
                InitializeScriptingEngine();
                CreateServer();
                LoadExtensionLibraries();
                InitializeVariables();
                InitializeCommands();
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
            //}
            //catch (Exception ex)
            //{
                //if (Debugger.IsAttached)
                //{
                //    //throw;
                //}
//#if !COGNITO
//                ServerManager.Logger.AddOutput("Internal server error: " + ex.Message, OutputLevel.Error);
//                Console.Write("Press any key to exit.");
//                Console.ReadKey();
//                SaveLog();
//#else
//                MessageBox.Show("Internal server error: " + ex.Message);
//                SaveLog();
//#endif

            //}
        }

        private static void InitializeScriptingEngine()
        {
            ServerManager.Logger.AddOutput("Starting scripting engine.", OutputLevel.Info);
            ServerManager.Logger.AddOutput("Initializing functions and variables.", OutputLevel.Info, "Scripting Engine");
            InitializeGlobals();
            ServerManager.ScriptBuilder.InitializeEngine();
            ServerManager.Logger.AddOutput($"Engine started. IronPython version {ServerManager.ScriptBuilder.ScriptingEngine.LanguageVersion.ToString()}", OutputLevel.Info, "Scripting Engine");
            ServerManager.Logger.AddOutput("Redirecting STDOUT to duplex channel.", OutputLevel.Debug, "Scripting Engine");
            ServerManager.ScriptBuilder.ScriptingEngine.Runtime.IO.SetOutput(new MemoryStream(), new Internal._ClientTextWriter());
            //ServerManager.ScriptBuilder.ScriptingEngine.Runtime.IO.SetInput(new MemoryStream(), new Internal._ClientTextReader(), Encoding.ASCII);
            ServerManager.Logger.AddOutput("Finished starting scripting engine.", OutputLevel.Info);
        }

        internal static void InitializeGlobals()
        {
            try
            {
                ServerManager.ScriptBuilder.AddScriptObject("serverInstance", new LuaServerInstance(), "Provides access to the global server instance.", ScriptGlobalType.Variable);
                ServerManager.ScriptBuilder.AddScriptObject("executeServerCommand", new Func<string, CommandPipeline>((command => ServerManager.ServerRemoteService.RemoteInterface.RunServerCommand(command, CommandExecutionMode.Script))), "Executes a command to the server.", ScriptGlobalType.Function);
                ServerManager.ScriptBuilder.AddScriptObject("speak", new Action<string, int, int>(StaticRemoteFunctions.speak), "Makes the server speak.", ScriptGlobalType.Function);
                ServerManager.ScriptBuilder.AddScriptObject("beep", new Action<int, int>(StaticRemoteFunctions.beep), "Makes the server beep.", ScriptGlobalType.Function);
                ServerManager.ScriptBuilder.AddScriptObject("functionExists", new Func<string, bool>((name) => ServerManager.ScriptBuilder.FunctionExists(name)), "Returns true if the function exists in the server.", ScriptGlobalType.Function);
                ServerManager.ScriptBuilder.AddScriptObject("createRequestBuilder", new Func<string, string, Dictionary<string, string>, RequestBuilder>(ClientInstance.createRequestBuilder), "Generates a request builder to be used to generate a request.", ScriptGlobalType.Function);
                ServerManager.ScriptBuilder.AddScriptObject("clientPrint", new Action<string>((text => ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole(text))), "Prints the text to the client-console", ScriptGlobalType.Function);
            }
            catch (ArgumentException)
            {

            }
        }

        private static void CreateServer()
        {
            var endpointAddress = "Remote";
            if(ServerManager.DefaultSettings.DiscoverySettings.DiscoveryBehavior == ProxyConnectionMode.Connect)
            {
                endpointAddress += $"/{Guid.NewGuid()}";
            }
            _remote = new RemoteImpl();
            var service = ServerRemotePlusService.Create(typeof(IRemote), _remote, ServerManager.DefaultSettings.PortNumber,endpointAddress, (m, o) => ServerManager.Logger.AddOutput(m, o), null);
            ServiceThrottlingBehavior throt = new System.ServiceModel.Description.ServiceThrottlingBehavior();
            throt.MaxConcurrentCalls = int.MaxValue;
            service.Host.Description.Behaviors.Add(throt);
            SetupFileTransferService();
            ServerManager.Logger.AddOutput("Attaching server events.", OutputLevel.Debug);
            service.HostClosed += Host_Closed;
            service.HostClosing += Host_Closing;
            service.HostFaulted += Host_Faulted;
            service.HostOpened += Host_Opened;
            service.HostOpening += Host_Opening;
            service.HostUnknownMessageReceived += Host_UnknownMessageReceived;
            OpenMex(service, ServerManager.FileTransferService);
            IOCContainer.Kernel.Bind<IRemotePlusService<ServerRemoteInterface>>().ToConstant(service);
        }

        private static void ProxyService_HostFaulted(object sender, EventArgs e)
        {
            ServerManager.Logger.AddOutput("The proxy server state has been transferred to the faulted state.", OutputLevel.Error);
        }

        private static void ProxyService_HostClosed(object sender, EventArgs e)
        {
            ServerManager.Logger.AddOutput("Proxy server closed.", OutputLevel.Info);
        }

        private static void ProxyService_HostOpened(object sender, EventArgs e)
        {
            ServerManager.Logger.AddOutput($"Proxy server opened on port {ServerManager.DefaultSettings.DiscoverySettings.Setup.DiscoveryPort}", OutputLevel.Info);
        }

        private static void OpenMex(IRemotePlusService<ServerRemoteInterface> mainService, IRemotePlusService<FileTransferServciceInterface> fileTransfer)
        {
            if(ServerManager.DefaultSettings.EnableMetadataExchange)
            {
                ServerManager.Logger.AddOutput(new LogItem(OutputLevel.Info, "NOTE: Metadata exchange is enabled on the server.", "Server Host" ) { Color = ConsoleColor.Cyan });
                System.ServiceModel.Channels.Binding mexBinding = MetadataExchangeBindings.CreateMexHttpBinding();
                ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
                smb.HttpGetEnabled = true;
                smb.HttpGetUrl = new Uri("http://0.0.0.0:9001/Mex");
                ServiceMetadataBehavior smb2 = new ServiceMetadataBehavior();
                smb.HttpGetEnabled = true;
                smb.HttpGetUrl = new Uri("http://0.0.0.0:9001/Mex2");
                mainService.Host.Description.Behaviors.Add(smb);
                fileTransfer.Host.Description.Behaviors.Add(smb2);
                mainService.Host.AddServiceEndpoint(typeof(IMetadataExchange), mexBinding, "http://0.0.0.0:9001/Mex");
                fileTransfer.Host.AddServiceEndpoint(typeof(IMetadataExchange), mexBinding, "http://0.0.0.0:9001/Mex2");
            }
        }

        private static void SetupFileTransferService()
        {
            IRemotePlusService<FileTransferServciceInterface> fts = null;
            ServerManager.Logger.AddOutput("Adding file transfer service.", OutputLevel.Info);
            var binding = _ConnectionFactory.BuildBinding();
            binding.TransferMode = TransferMode.Streamed;
            fts = FileTransferService.CreateNotSingle(typeof(IFileTransferContract), ServerManager.DefaultSettings.PortNumber, binding, "FileTransfer", null);
            fts.HostClosed += Host_Closed;
            fts.HostClosing += Host_Closing;
            fts.HostFaulted += Host_Faulted;
            fts.HostOpened += Host_Opened;
            fts.HostOpening += Host_Opening;
            fts.HostUnknownMessageReceived += Host_UnknownMessageReceived;
            IOCContainer.Kernel.Bind<IRemotePlusService<FileTransferServciceInterface>>().ToConstant(fts);
        }

        private static void SaveLog()
        {
            try
            {
                if (ServerManager.DefaultSettings.LoggingSettings.LogOnShutdown)
                {
                    ServerManager.Logger.AddOutput("Saving log and closing.", OutputLevel.Info);
                    ServerManager.Logger.SaveLog($"{ServerManager.DefaultSettings.LoggingSettings.LogFolder}\\{DateTime.Now.ToShortDateString().Replace('/', ServerManager.DefaultSettings.LoggingSettings.DateDelimiter)} {DateTime.Now.ToShortTimeString().Replace(':', ServerManager.DefaultSettings.LoggingSettings.TimeDelimiter)}.txt");
                }
            }
            catch (Exception ex)
            {
                ServerManager.Logger.AddOutput($"Unable to save log file: {ex.Message}", OutputLevel.Error);
            }
        }

        private static void InitalizeKnownTypes()
        {
            ServerManager.Logger.AddOutput("Adding default known types.", OutputLevel.Info);
            DefaultKnownTypeManager.LoadDefaultTypes();
            ServerManager.Logger.AddOutput("Adding UserAccount to known type list.", OutputLevel.Debug);
            DefaultKnownTypeManager.AddType(typeof(UserAccount));
        }

        private static void InitializeVariables()
        {
            if(File.Exists("Variables.xml"))
            {
                ServerManager.Logger.AddOutput("Loading variables.", OutputLevel.Info);
                ServerManager.ServerRemoteService.Variables = VariableManager.Load();
            }
            else
            {
                ServerManager.Logger.AddOutput("There is no variable file. Beginning variable initialization.", OutputLevel.Warning);
                ServerManager.ServerRemoteService.Variables = VariableManager.New();
                ServerManager.ServerRemoteService.Variables.Add("Name", "RemotePlusServer");
                ServerManager.Logger.AddOutput("Saving file.", OutputLevel.Info);
                ServerManager.ServerRemoteService.Variables.Save();
            }
        }

        private static void InitializeCommands()
        {
            ServerManager.Logger.AddOutput("Loading Commands.", OutputLevel.Info);
            ServerManager.ServerRemoteService.Commands.Add("ps", ProcessStartCommand);
            ServerManager.ServerRemoteService.Commands.Add("help", Help);
            ServerManager.ServerRemoteService.Commands.Add("logs", Logs);
            ServerManager.ServerRemoteService.Commands.Add("vars", vars);
            ServerManager.ServerRemoteService.Commands.Add("dateTime", dateTime);
            ServerManager.ServerRemoteService.Commands.Add("processes", processes);
            ServerManager.ServerRemoteService.Commands.Add("version", version);
            ServerManager.ServerRemoteService.Commands.Add("encrypt", svm_encyptFile);
            ServerManager.ServerRemoteService.Commands.Add("decrypt", svm_decryptFile);
            ServerManager.ServerRemoteService.Commands.Add("beep", svm_beep);
            ServerManager.ServerRemoteService.Commands.Add("speak", svm_speak);
            ServerManager.ServerRemoteService.Commands.Add("showMessageBox", svm_showMessageBox);
            ServerManager.ServerRemoteService.Commands.Add("path", path);
            ServerManager.ServerRemoteService.Commands.Add("cd", cd);
            ServerManager.ServerRemoteService.Commands.Add("echo", echo);
            ServerManager.ServerRemoteService.Commands.Add("load-extensionLibrary", loadExtensionLibrary);
            ServerManager.ServerRemoteService.Commands.Add("cp", cp);
            ServerManager.ServerRemoteService.Commands.Add("deleteFile", deleteFile);
            ServerManager.ServerRemoteService.Commands.Add("echoFile", echoFile);
            ServerManager.ServerRemoteService.Commands.Add("ls", ls);
            ServerManager.ServerRemoteService.Commands.Add("genMan", genMan);
            ServerManager.ServerRemoteService.Commands.Add("scp", scp);
            ServerManager.ServerRemoteService.Commands.Add("resetStaticScript", resetStaticScript);
            ServerManager.ServerRemoteService.Commands.Add("requestFile", requestFile);
        }

        static bool CheckPrerequisites()
        {
            ServerManager.Logger.AddOutput("Checking prerequisites.", OutputLevel.Info);
            //Check for prerequisites
            ServerPrerequisites.CheckPrivilleges();
            ServerPrerequisites.CheckNetworkInterfaces();
            ServerPrerequisites.CheckSettings();
            ServerManager.Logger.AddOutput("Stopping stop watch.", OutputLevel.Debug);
            sw.Stop();
            // Check results
            if(ServerManager.Logger.errorcount >= 1 && ServerManager.Logger.warningcount == 0)
            {
                ServerManager.Logger.AddOutput($"Unable to start server. ({ServerManager.Logger.errorcount} errors) Elapsed time: {sw.Elapsed.ToString()}", OutputLevel.Error);
                return false;
            }
            else if(ServerManager.Logger.errorcount >= 1 && ServerManager.Logger.warningcount >= 1)
            {
                ServerManager.Logger.AddOutput($"Unable to start server. ({ServerManager.Logger.errorcount} errors, {ServerManager.Logger.warningcount} warnings) Elapsed time: {sw.Elapsed.ToString()}", OutputLevel.Error);
                return false;
            }
            else if(ServerManager.Logger.errorcount == 0 && ServerManager.Logger.warningcount >= 1)
            {
                ServerManager.Logger.AddOutput($"The server can start, but with warnings. ({ServerManager.Logger.warningcount} warnings) Elapsed time: {sw.Elapsed.ToString()}", OutputLevel.Warning);
                return true;
            }
            else
            {
                ServerManager.Logger.AddOutput(new LogItem(OutputLevel.Info, $"Validation passed. Elapsed time: {sw.Elapsed.ToString()}", "Server Host") { Color = ConsoleColor.Green });
                return true;
            }
        }
        static void LoadExtensionLibraries()
        {
            List<string> excludedFiles = new List<string>();
            ServerManager.Logger.AddOutput("Loading extensions...", Logging.OutputLevel.Info);
            if (Directory.Exists("extensions"))
            {
                if (File.Exists("extensions\\excludes.txt"))
                {
                    ServerManager.Logger.AddOutput("Found an excludes.txt file. Reading file...", OutputLevel.Info);
                    foreach(string excludedFile in File.ReadLines("extensions\\excludes.txt"))
                    {
                        ServerManager.Logger.AddOutput($"{excludedFile} is excluded from the extension search.", OutputLevel.Info);
                        excludedFiles.Add("extensions\\" + excludedFile);
                    }
                    ServerManager.Logger.AddOutput("Finished reading extension exclusion file.", OutputLevel.Info);
                }
                ServerInitEnvironment env = new ServerInitEnvironment(false);
                foreach (string files in Directory.GetFiles("extensions"))
                {
                    if (Path.GetExtension(files) == ".dll" && !excludedFiles.Contains(files))
                    {
                        try
                        {
                            ServerManager.Logger.AddOutput($"Found extension file ({Path.GetFileName(files)})", Logging.OutputLevel.Info);
                            env.PreviousError = ServerManager.Logger.errorcount > 0 ? true : false;
                            var lib = ServerExtensionLibrary.LoadServerLibrary(files, (m, o) => ServerManager.Logger.AddOutput(m, o), env);
                            ServerManager.DefaultCollection.Libraries.Add(lib.Name, lib);
                        }
                        catch (Exception ex)
                        {
                            ServerManager.Logger.AddOutput($"Could not load \"{files}\" because of a load error or initialization error. Error: {ex.Message}", OutputLevel.Warning);
                        }
                        env.InitPosition++;
                    }
                }
                ServerManager.Logger.AddOutput($"{ServerManager.DefaultCollection.Libraries.Count} extension libraries loaded.", OutputLevel.Info);
            }
            else
            {
                ServerManager.Logger.AddOutput("The extensions folder does not exist.", OutputLevel.Info);
            }
        }
        public static DuplexChannelFactory<IProxyServerRemote> proxyChannelFactory = null;
        public static IProxyServerRemote proxyChannel = null;
        public static void RunInServerMode()
        {
            if (ServerManager.DefaultSettings.DiscoverySettings.DiscoveryBehavior == ProxyConnectionMode.Connect)
            {
                ServerManager.Logger.AddOutput("The server will be part of a proxy cluster. Please use the proxy server to connect to this server.", OutputLevel.Info);
                proxyChannelFactory = new DuplexChannelFactory<IProxyServerRemote>(_remote, _ConnectionFactory.BuildBinding(), new EndpointAddress(ServerManager.DefaultSettings.DiscoverySettings.Connection.ProxyServerURL));
                proxyChannel = proxyChannelFactory.CreateChannel();
                proxyChannel.Register();
            }
            else
            {
                ServerManager.ServerRemoteService.Start();
                ServerManager.FileTransferService.Start();
            }
        }

        private static void Host_UnknownMessageReceived(object sender, UnknownMessageReceivedEventArgs e)
        {
            ServerManager.Logger.AddOutput($"The server encountered an unknown message sent by the client. Message: {e.Message.ToString()}", OutputLevel.Error);
        }

        private static void Host_Opening(object sender, EventArgs e)
        {
            ServerManager.Logger.AddOutput("Opening server.", OutputLevel.Info);
        }

        private static void Host_Opened(object sender, EventArgs e)
        {
            if (ServerManager.DefaultSettings.DiscoverySettings.DiscoveryBehavior == ProxyConnectionMode.Connect)
            {
                ServerManager.Logger.AddOutput($"Host ready. Server is now part of the proxy cluster. Connect to proxy server to configure this server.", OutputLevel.Info);
            }
            else
            {
                ServerManager.Logger.AddOutput($"Host ready. Server is listening on port {ServerManager.DefaultSettings.PortNumber}. Connect to configure server.", Logging.OutputLevel.Info);
            }
        }

        private static void Host_Faulted(object sender, EventArgs e)
        {
            ServerManager.Logger.AddOutput("The server state has been transferred to the faulted state.", OutputLevel.Error);
        }

        private static void Host_Closing(object sender, EventArgs e)
        {
            ServerManager.Logger.AddOutput("Closing the server.", OutputLevel.Info);
        }

        private static void Host_Closed(object sender, EventArgs e)
        {
            ServerManager.Logger.AddOutput("The server is now closed.", OutputLevel.Info);
        }

        static void ScanForServerSettingsFile()
        {
            if (!File.Exists("Configurations\\Server\\Roles.config"))
            {
                buildAdminPolicyObject();
                Role.InitializeRolePool();
                ServerManager.Logger.AddOutput("The server roles file does not exist. Creating server roles settings file.", OutputLevel.Warning);
                var r = Role.CreateRole("Administrators");
                Role.GlobalPool.Roles.Add(r);
                DefaultKnownTypeManager.AddType(typeof(OperationPolicies));
                DefaultKnownTypeManager.AddType(typeof(DefaultPolicy));
                Role.GlobalPool.Save();
            }
            else
            {
                ServerManager.Logger.AddOutput("Loading server roles file.", OutputLevel.Info);
                try
                {
                    DefaultKnownTypeManager.AddType(typeof(OperationPolicies));
                    DefaultKnownTypeManager.AddType(typeof(DefaultPolicy));
                    Role.GlobalPool.Load();
                }
                catch (Exception ex)
                {
#if DEBUG
                    ServerManager.Logger.AddOutput("Unable to load server settings. " + ex.ToString(), OutputLevel.Error);
#else
                    ServerManager.Logger.AddOutput("Unable to load server settings. " + ex.Message, OutputLevel.Error);
#endif
                }
            }
            if(!Directory.Exists("Users"))
            {
                ServerManager.Logger.AddOutput("The Users folder does not exist. Creating folder.", OutputLevel.Warning);
                Directory.CreateDirectory("Users");
                AccountManager.CreateAccount(new UserCredentials("admin", "password"), "Administrators");
            }
            else
            {
                AccountManager.RefreshAccountList();
            }
            ServerManager.DefaultSettings = new ServerSettings();
            if (!File.Exists("Configurations\\Server\\GlobalServerSettings.config"))
            {
                ServerManager.Logger.AddOutput("The server settings file does not exist. Creating server settings file.", OutputLevel.Warning);
                ServerManager.DefaultSettings.Save();
            }
            else
            {
                ServerManager.Logger.AddOutput("Loading server settings file.", OutputLevel.Info);
                try
                {
                    ServerManager.DefaultSettings.Load();
                }
                catch (Exception ex)
                {
#if DEBUG
                    ServerManager.Logger.AddOutput("Unable to load server settings. " + ex.ToString(), OutputLevel.Error);
#else
                    ServerManager.Logger.AddOutput("Unable to load server settings. " + ex.Message, OutputLevel.Error);
#endif
                }
            }
        }

        private static void buildAdminPolicyObject()
        {
            var policies = new OperationPolicies();
            policies.EnableConsole = true;
            PolicyObject adminObject = new PolicyObject("Admin");
            adminObject.Policies.Folders.Add(policies);
            adminObject.Save();
        }

        public static void Close()
        {
            SaveLog();
            ServerManager.ServerRemoteService.Host.Close();
            ServerManager.FileTransferService.Close();
            if(ServerManager.DefaultSettings.DiscoverySettings.DiscoveryBehavior == ProxyConnectionMode.Connect && proxyChannelFactory != null)
            {
                proxyChannel.Leave(ServerGuid);
                proxyChannelFactory.Close();
            }
            Environment.Exit(0);
        }
    }
}