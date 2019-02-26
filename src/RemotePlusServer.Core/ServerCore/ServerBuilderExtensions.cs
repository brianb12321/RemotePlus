using BetterLogger;
using RemotePlusLibrary.Configuration.ServerSettings;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.Scripting;
using RemotePlusLibrary.Security.AccountSystem;
using RemotePlusServer.Core.ExtensionSystem;
using RemotePlusServer.Core.Proxies;
using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel.Description;
using System.Speech.Synthesis;
using RemotePlusLibrary.Extension.ResourceSystem;
using RemotePlusLibrary.Extension.ResourceSystem.ResourceTypes;
using RemotePlusLibrary.Extension.ResourceSystem.ResourceTypes.Devices;
using System.Linq;
using System.Reflection;

namespace RemotePlusServer.Core.ServerCore
{
    /// <summary>
    /// Provides helper methods for server core extensions to use to setup a custom server.
    /// </summary>
    public static class ServerBuilderExtensions
    {
        /// <summary>
        /// Preloads <see cref="ServerSettings"/> with the perferred settings. NOTE: not calling <see cref="SkipServerSettingsLoading(IServerBuilder)"/> will override your preferred settings.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServerBuilder PreloadSettings(this IServerBuilder builder, Action<ServerSettings> options)
        {
            return builder.AddTask(() =>
            {
                ServerSettings s = new ServerSettings();
                options?.Invoke(s);
                ServerManager.DefaultSettings = s;
            });
        }
        public static IServerBuilder ResolveLib(this IServerBuilder builder)
        {
            return builder.AddTask(() =>
            {
                AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            });
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs e)
        {
            string name = e.RequestingAssembly.GetName().Name;
            if (Directory.Exists("Libs") && Directory.Exists($"Libs\\{name}"))
            {
                var assembly = Assembly.LoadFrom($"Libs\\{name}\\{e.Name}");
                return assembly;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Sets the path to load the server settings.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="path">The path to set the search path to.</param>
        /// <returns></returns>
        public static IServerBuilder SetupServerConfigPath(this IServerBuilder builder, string path)
        {
            return builder.AddTask(() =>
            {
                ServerInitiliazationPipeline.ServerConfigPath = path;
            });
        }
        /// <summary>
        /// Initializes the default known types used in RemotePlus
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IServerBuilder InitializeServerSideKnownTypes(this IServerBuilder builder)
        {
            return builder.AddTask(() =>
            {
                
            });
        }
        /// <summary>
        /// Initializes all the variables and functions pre-installed into RemotePlus scripting engine.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IServerBuilder InitializeDefaultGlobals(this IServerBuilder builder)
        {
            return builder.AddTask(() =>
            {
                GlobalServices.Logger.Log("Initializing functions and variables.", LogLevel.Info, "Scripting Engine");
                InitializeGlobals();
            });
        }
        public static IServerBuilder LoadExtensionLibraries(this IServerBuilder builder)
        {
            return builder.AddTask(() =>
            {
                ServerExtensionLibraryCollection.LoadExtensionsInFolder();
            });
        }
        public static void InitializeGlobals()
        {
            try
            {
                ServerManager.ScriptBuilder.AddScriptObject("serverInstance", new PythonServerInstance(), "Provides access to the global server instance.", ScriptGlobalType.Variable);
                ServerManager.ScriptBuilder.AddScriptObject("executeServerCommand", new Func<string, CommandPipeline>((command => ServerManager.ServerRemoteService.RemoteInterface.RunServerCommand(command, CommandExecutionMode.Script))), "Executes a command to the server.", ScriptGlobalType.Function);
                ServerManager.ScriptBuilder.AddScriptObject("speak", new Action<string, VoiceGender, VoiceAge>(StaticRemoteFunctions.speak), "Makes the server speak.", ScriptGlobalType.Function);
                ServerManager.ScriptBuilder.AddScriptObject("beep", new Action<int, int>(StaticRemoteFunctions.beep), "Makes the server beep.", ScriptGlobalType.Function);
                ServerManager.ScriptBuilder.AddScriptObject("functionExists", new Func<string, bool>((name) => ServerManager.ScriptBuilder.FunctionExists(name)), "Returns true if the function exists in the server.", ScriptGlobalType.Function);
                ServerManager.ScriptBuilder.AddScriptObject("clientPrint", new Action<string>((text => ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole(text))), "Prints the text to the client-console", ScriptGlobalType.Function);
                ServerManager.ScriptBuilder.AddScriptObject("ps", new Action<string, string, bool, bool>((program, args, shell, ignore) =>
                {
                    ServerManager.ServerRemoteService.RemoteInterface.RunProgram(program, args, shell, ignore);
                }), "Executes a program on the server using the RunProgram service method.", ScriptGlobalType.Function);
            }
            catch (ArgumentException)
            {

            }
        }
        /// <summary>
        /// Initializes the scripting engine used for executing scripts.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="options">Configuration options for the scripting engine.</param>
        /// <returns></returns>
        public static IServerBuilder InitializeScriptingEngine(this IServerBuilder builder, Action<ScriptingEngineOptions> options)
        {
            return builder.AddTask(() =>
            {
                GlobalServices.Logger.Log("Starting scripting engine.", LogLevel.Info);
                ServerManager.ScriptBuilder.InitializeEngine();
                GlobalServices.Logger.Log($"Engine started. IronPython version {ServerManager.ScriptBuilder.ScriptingEngine.LanguageVersion.ToString()}", LogLevel.Info, "Scripting Engine");
                //ServerManager.ScriptBuilder.ScriptingEngine.Runtime.IO.SetInput(new MemoryStream(), new Internal._ClientTextReader(), Encoding.ASCII);
                GlobalServices.Logger.Log("Setting sys.path.", LogLevel.Debug, "Scripting Engine");
                ICollection<string> paths = ServerManager.ScriptBuilder.ScriptingEngine.GetSearchPaths();
                paths.Add(AppDomain.CurrentDomain.BaseDirectory + "\\PythonStdLib");
                ServerManager.ScriptBuilder.ScriptingEngine.SetSearchPaths(paths);
                GlobalServices.Logger.Log("Finished starting scripting engine.", LogLevel.Info);
            });
        }
        /// <summary>
        /// Sets up the variable system for RemotePlus server.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IServerBuilder InitializeVariables(this IServerBuilder builder)
        {
            return builder.AddTask(() =>
            {
                IResourceManager resourceManager = IOCContainer.GetService<IResourceManager>();
                resourceManager.AddResource("/serverProperties", new StringResource("Name", "RemotePlusServer"));
                IDeviceSearcher searcher = new DefaultDeviceSearcher();
                resourceManager.AddResource("/dev/utils", new TTSDevice());
                searcher.Get<KeyboardDevice>("keyboard").ToList().ForEach(d => resourceManager.AddResource("/dev/io", d));
                searcher.Get<MouseDevice>("mouse").ToList().ForEach(d => resourceManager.AddResource("/dev/io", d));
                ServerManager.ScriptBuilder.AddScriptObject<Func<string, ResourceQuery>>("resq", (name) => new ResourceQuery(name, Guid.Empty), "Generates a new resource query.", ScriptGlobalType.Function);
            });
        }
        /// <summary>
        /// Reads the server config file and loads the settings defined in the file.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IServerBuilder LoadServerConfig(this IServerBuilder builder)
        {
            return builder.AddTask(() =>
            {
                ServerManager.DefaultSettings = new ServerSettings();
                if (!File.Exists("Configurations\\Server\\GlobalServerSettings.config"))
                {
                    ServerSettings internalSettings = loadConfigFromAssembly();
                    if(internalSettings != null)
                    {
                        GlobalServices.Logger.Log("Server settings loaded from assembly.", LogLevel.Info);
                        ServerManager.DefaultSettings = internalSettings;
                    }
                    else
                    {
                        GlobalServices.Logger.Log("The server settings file does not exist. Creating server settings file.", LogLevel.Warning);
                        ServerManager.DataAccess.SaveConfig(ServerManager.DefaultSettings, ServerSettings.SERVER_SETTINGS_FILE_PATH);
                    }
                }
                else
                {
                    GlobalServices.Logger.Log("Loading server settings file.", LogLevel.Info);
                    try
                    {
                        ServerManager.DefaultSettings = ServerManager.DataAccess.LoadConfig<ServerSettings>(ServerSettings.SERVER_SETTINGS_FILE_PATH);
                    }
                    catch (Exception ex)
                    {
#if DEBUG
                        GlobalServices.Logger.Log("Unable to load server settings. " + ex.ToString(), LogLevel.Error);
#else
                    GlobalServices.Logger.Log("Unable to load server settings. " + ex.Message, LogLevel.Error);
#endif
                    }
                }
                if (!Directory.Exists("Users"))
                {
                    const string DEFAULT_USERNAME = "admin";
                    const string DEFAULT_PASSWORD = "password";
                    if(ServerManager.DefaultSettings.UseDefaultUserIfNoneExists)
                    {
                        GlobalServices.Logger.Log("The user folder does not exist. Using default user.", LogLevel.Info);
                        ServerManager.AccountManager.CreateAccount(new UserCredentials(DEFAULT_USERNAME, DEFAULT_PASSWORD), false);
                    }
                    else
                    {
                        GlobalServices.Logger.Log("The Users folder does not exist. Creating folder.", LogLevel.Warning);
                        Directory.CreateDirectory("Users");
                        ServerManager.AccountManager.CreateAccount(new UserCredentials(DEFAULT_USERNAME, DEFAULT_PASSWORD));
                    }
                }
                else
                {
                    ServerManager.AccountManager.RefreshAccountList();
                }
            });
        }

        private static ServerSettings loadConfigFromAssembly()
        {
            try
            {
                Assembly loadedAssembly = Assembly.GetEntryAssembly();
                GlobalServices.Logger.Log($"Scanning for embedded config file in assembly: {loadedAssembly.GetName()}", LogLevel.Debug);
                var stream = loadedAssembly.GetManifestResourceStream("RemotePlusServer.InternalConfig.config");
                return ServerManager.DataAccess.LoadConfig<ServerSettings>(stream);
            }
            catch (Exception ex)
            {
                GlobalServices.Logger.Log($"Unable to open internal config file: {ex.Message}", LogLevel.Debug);
                return null;
            }
        }

        public static IServerBuilder OpenMexForRemotePlus(this IServerBuilder builder)
        {
            return builder.AddTask(() =>
            {
                if (ServerManager.DefaultSettings.EnableMetadataExchange)
                {
                    GlobalServices.Logger.Log("NOTE: Metadata exchange is enabled on the server.", LogLevel.Info, "Server Host");
                    System.ServiceModel.Channels.Binding mexBinding = MetadataExchangeBindings.CreateMexTcpBinding();
                    ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
                    smb.HttpGetEnabled = true;
                    smb.HttpGetUrl = new Uri("http://0.0.0.0:9001/Mex");
                    ServerManager.ServerRemoteService.Host.Description.Behaviors.Add(smb);
                    ServerManager.ServerRemoteService.Host.AddServiceEndpoint(typeof(IMetadataExchange), mexBinding, "http://0.0.0.0:9001/Mex");
                }
            });
        }
        public static IServerBuilder OpenMexForFileTransfer(this IServerBuilder builder)
        {
            return builder.AddTask(() =>
            {
                if (ServerManager.DefaultSettings.EnableMetadataExchange)
                {
                    System.ServiceModel.Channels.Binding mexBinding = MetadataExchangeBindings.CreateMexTcpBinding();
                    ServiceMetadataBehavior smb2 = new ServiceMetadataBehavior();
                    smb2.HttpGetEnabled = true;
                    smb2.HttpGetUrl = new Uri("http://0.0.0.0:9001/Mex2");
                    ServerManager.FileTransferService.Host.Description.Behaviors.Add(smb2);
                    ServerManager.FileTransferService.Host.AddServiceEndpoint(typeof(IMetadataExchange), mexBinding, "http://0.0.0.0:9001/Mex2");
                }
            });
        }
    }
}