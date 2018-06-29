using Logging;
using RemotePlusLibrary;
using RemotePlusLibrary.Configuration.ServerSettings;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using RemotePlusLibrary.RequestSystem;
using RemotePlusLibrary.Scripting;
using RemotePlusLibrary.Security.AccountSystem;
using RemotePlusLibrary.Security.AccountSystem.Policies;
using RemotePlusServer.Core.ExtensionSystem;
using RemotePlusServer.Core.Proxies;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public static IServerBuilder InitializeKnownTypes(this IServerBuilder builder)
        {
            return builder.AddTask(() =>
            {
                ServerManager.Logger.AddOutput("Adding default known types.", OutputLevel.Info);
                DefaultKnownTypeManager.LoadDefaultTypes();
                ServerManager.Logger.AddOutput("Adding UserAccount to known type list.", OutputLevel.Debug);
                DefaultKnownTypeManager.AddType(typeof(UserAccount));
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
                ServerManager.Logger.AddOutput("Initializing functions and variables.", OutputLevel.Info, "Scripting Engine");
                InitializeGlobals();
            });
        }
        public static void InitializeGlobals()
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
                ServerManager.Logger.AddOutput("Starting scripting engine.", OutputLevel.Info);
                ServerManager.ScriptBuilder.InitializeEngine();
                ServerManager.Logger.AddOutput($"Engine started. IronPython version {ServerManager.ScriptBuilder.ScriptingEngine.LanguageVersion.ToString()}", OutputLevel.Info, "Scripting Engine");
                ServerManager.Logger.AddOutput("Redirecting STDOUT to duplex channel.", OutputLevel.Debug, "Scripting Engine");
                ServerManager.ScriptBuilder.ScriptingEngine.Runtime.IO.SetOutput(new MemoryStream(), new Internal._ClientTextWriter());
                //ServerManager.ScriptBuilder.ScriptingEngine.Runtime.IO.SetInput(new MemoryStream(), new Internal._ClientTextReader(), Encoding.ASCII);
                ServerManager.Logger.AddOutput("Finished starting scripting engine.", OutputLevel.Info);
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
                if (File.Exists("Variables.xml"))
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
            });
        }
        /// <summary>
        /// Adds a command to the default server.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="commandName">the name of the command for which the user will type to execute the command.</param>
        /// <param name="command">The command itself</param>
        /// <returns></returns>
        public static IServerBuilder AddCommand(this IServerBuilder builder, string commandName, CommandDelegate command)
        {
            return builder.AddTask(() =>
            {
                ServerManager.ServerRemoteService.Commands.Add(commandName, command);
            });
        }
        /// <summary>
        /// Searches for extension libraries to load.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IServerBuilder LoadExtensionLibraries(this IServerBuilder builder)
        {
            return builder.AddTask(() =>
            {
                List<string> excludedFiles = new List<string>();
                ServerManager.Logger.AddOutput("Loading extensions...", Logging.OutputLevel.Info);
                if (Directory.Exists("extensions"))
                {
                    if (File.Exists("extensions\\excludes.txt"))
                    {
                        ServerManager.Logger.AddOutput("Found an excludes.txt file. Reading file...", OutputLevel.Info);
                        foreach (string excludedFile in File.ReadLines("extensions\\excludes.txt"))
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
                if (!File.Exists(ServerInitiliazationPipeline.ServerConfigPath))
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
                if (!Directory.Exists("Users"))
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
            });
        }
        private static void buildAdminPolicyObject()
        {
            var policies = new OperationPolicies();
            policies.EnableConsole = true;
            PolicyObject adminObject = new PolicyObject("Admin");
            adminObject.Policies.Folders.Add(policies);
            adminObject.Save();
        }
    }
}
