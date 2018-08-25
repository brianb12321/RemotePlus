using System;
using System.Collections.Generic;
using RemotePlusLibrary;
using System.ServiceModel;
using RemotePlusLibrary.Core;
using System.Windows.Forms;
using System.Drawing;
using RemotePlusClient.CommonUI;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using System.Text.RegularExpressions;
using RemotePlusLibrary.Security.AccountSystem;
using RemotePlusClient.CommonUI.ConnectionClients;
using RemotePlusLibrary.Core.Faults;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses.Parsing;
using BetterLogger;
using BetterLogger.Loggers;
using System.IO;
using RemotePlusClientCmd.ClientExtensionSystem;
using RemotePlusLibrary.Core.IOC;
using RemotePlusClient.CommonUI.Connection;
using RemotePlusLibrary.Extension.EventSystem;

namespace RemotePlusClientCmd
{
    public partial class ClientCmdManager
    {
        public static Dictionary<string, CommandDelegate> LocalCommands = new Dictionary<string, CommandDelegate>();
        public static ClientExtensionLibraryCollection ExtensionLibraries { get; set; } = new ClientExtensionLibraryCollection();
        public static ServiceClient Remote = null;
        public static PromptBuilder prompt = new PromptBuilder();
        public static ProxyClient Proxy = null;
        public static Connection CurrentConnectionData => IOCContainer.GetService<Connection>();
        public static IEventBus EventBus => IOCContainer.GetService<IEventBus>();
        public static bool WaitFlag = true;
        public static bool ProxyEnabled { get; private set; }
        [STAThread]
        static void Main(string[] args)
        {
            IOCContainer.Provider.Bind<Connection>().ToSelf().InSingletonScope();
            IOCContainer.Provider.Bind<RemotePlusLibrary.Configuration.IConfigurationDataAccess>().To(typeof(RemotePlusLibrary.Configuration.StandordDataAccess.ConfigurationHelper)).InSingletonScope().Named("DefaultConfigDataAccess");
            IOCContainer.Provider.Bind<IEventBus>().To(typeof(EventBus)).InSingletonScope();
            var blf = new BaseLogFactory();
            blf.AddLogger(new ConsoleLogger());
            IOCContainer.Provider.Bind<ILogFactory>().ToConstant(blf);
            Console.ResetColor();
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            //Application.ThreadException += (Sender, e) => CatchException(e.Exception);
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                if (e.ExceptionObject is FaultException<ServerFault>)
                {
                    CatchException((FaultException<ServerFault>)e.ExceptionObject);
                }
                else
                {
                    CatchException((Exception)e.ExceptionObject);
                }
                if(e.IsTerminating)
                {
                    Environment.Exit(-1);
                }
            };
            ShowBanner();
            LoadExtensions();
            InitCommands();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            InitializeDefaultKnownTypes();
            RequestStore.Init();
            RequestStore.Add("rcmd_menu", new Requests.ConsoleMenuRequest());
            RequestStore.Add("rcmd_smenu", new Requests.SelectableConsoleMenu());
            RequestStore.Add("rcmd_messageBox", new Requests.RCmdMessageBox());
            RequestStore.Add("rcmd_textBox", new Requests.RCmdTextBox());
            RequestStore.Add("rcmd_multitextBox", new Requests.RCmdMultiLineTextbox());
            RequestStore.Add("global_selectFile", new Requests.SelectFileRequest());
            RequestStore.Add("global_sendFilePackage", new Requests.SendFilePackageRequest());
            if (args.Length == 0)
            {
                Console.Write("Enter url: ");
                string url = Console.ReadLine();
                Console.Write("Enter Username: ");
                string username = Console.ReadLine();
                Console.Write("Enter Password: ");
                string password = Console.ReadLine();
                RegisterationObject ro = new RegisterationObject();
                {
                    ro.LoginRightAway = true;
                    ro.Credentials = new UserCredentials(username, password);
                };
                Connect(url, ro, false);
                AcceptInput(false);
            }
            else
            {
                var options = new CommandLineOptions();
                if (CommandLine.Parser.Default.ParseArguments(args, options))
                {
                    Connect(options.Url, new RegisterationObject() { LoginRightAway = true, Credentials = new UserCredentials(options.Username, options.Password), VerboseError = options.Verbose }, options.UseProxy);
                    ProxyEnabled = options.UseProxy;
                    AcceptInput(options.UseProxy);
                }
            }
        }

        private static void LoadExtensions()
        {
            List<string> excludedFiles = new List<string>();
            GlobalServices.Logger.Log("Loading extensions...", LogLevel.Info);
            if (Directory.Exists("extensions"))
            {
                if (File.Exists("extensions\\excludes.txt"))
                {
                    GlobalServices.Logger.Log("Found an excludes.txt file. Reading file...", LogLevel.Info);
                    foreach (string excludedFile in File.ReadLines("extensions\\excludes.txt"))
                    {
                        GlobalServices.Logger.Log($"{excludedFile} is excluded from the extension search.", LogLevel.Info);
                        excludedFiles.Add("extensions\\" + excludedFile);
                    }
                    GlobalServices.Logger.Log("Finished reading extension exclusion file.", LogLevel.Info);
                }
                ClientInitEnvironment env = new ClientInitEnvironment(false);
                foreach (string files in Directory.GetFiles("extensions"))
                {
                    if (Path.GetExtension(files) == ".dll" && !excludedFiles.Contains(files))
                    {
                        try
                        {
                            GlobalServices.Logger.Log($"Found extension file ({Path.GetFileName(files)})", LogLevel.Info);
                            env.PreviousError = GlobalServices.Logger.ErrorCount > 0 ? true : false;
                            var lib = ClientExtensionLibrary.LoadClientLibrary(files, (m, o) => GlobalServices.Logger.Log(m, o), env);
                            ExtensionLibraries.Libraries.Add(lib.Name, lib);
                        }
                        catch (Exception ex)
                        {
                            GlobalServices.Logger.Log($"Could not load \"{files}\" because of a load error or initialization error. Error: {ex.Message}", LogLevel.Warning);
                        }
                        env.InitPosition++;
                    }
                }
                GlobalServices.Logger.Log($"{ExtensionLibraries.Libraries.Count} extension libraries loaded.", LogLevel.Info);
            }
            else
            {
                GlobalServices.Logger.Log("The extensions folder does not exist.", LogLevel.Info);
            }
        }

        static void CatchException(Exception ex)
        {
            using (ErrorDialog d = new ErrorDialog(ex))
            {
                d.ShowDialog();
            }
        }
        static void Connect(string url, RegisterationObject ro, bool useProxy)
        {
            if (useProxy)
            {
                var ea = new EndpointAddress(url);
                CurrentConnectionData.BaseAddress = ea.Uri.Host;
                CurrentConnectionData.Port = ea.Uri.Port;
                Proxy = new ProxyClient(new ClientCallback(), _ConnectionFactory.BuildBinding(), ea);
                Proxy.ChannelFactory.Faulted += (sender, e) =>
                {
                    var dr = MessageBox.Show("The connection to the proxy server has faulted. Would you like to reconnect to the server.", "RemotePlusClientCmd", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                    if(dr == DialogResult.Yes)
                    {
                        Proxy = new ProxyClient(new ClientCallback(), _ConnectionFactory.BuildBinding(), ea);
                        WaitFlag = false;
                        Proxy.ProxyRegister();
                    }
                };
                Proxy.ProxyRegister();
            }
            else
            {
                var ea = new EndpointAddress(url);
                CurrentConnectionData.BaseAddress = ea.Uri.Host;
                CurrentConnectionData.Port = ea.Uri.Port;
                Remote = new ServiceClient(new ClientCallback(), _ConnectionFactory.BuildBinding(), ea);
                Remote.Register(ro);
            }
        }
        static void AcceptInput(bool useProxy)
        {
            Console.WriteLine("Enter a command to the server. Type {help} for a list of commands.");
            while (true)
            {
                if (!WaitFlag)
                {
                    WritePrompt();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    var c = Console.ReadLine();
                    Console.ResetColor();
                    if(string.IsNullOrEmpty(c))
                    {
                        c = " ";
                    }
                    if (c.ToCharArray()[0] == '#')
                    {
                        int pos = 0;
                        CommandPipeline pipe = new CommandPipeline();
                        CommandParser parser = new CommandParser();
                        parser.ResetCommand(c);
                        var tokens = parser.Parse(true);
                        var newTokens = RunSubRoutines(parser, pipe, pos);
                        foreach (CommandToken token in newTokens)
                        {
                            foreach (List<CommandToken> allTokens in parser.ParsedTokens)
                            {
                                var index = allTokens.IndexOf(token);
                                if (index != -1)
                                {
                                    parser.ParsedTokens[parser.ParsedTokens.IndexOf(allTokens)][index] = token;
                                }
                            }
                        }
                        //var newVariableTokens = RunVariableReplacement(parser, out bool success);
                        //if (success != true)
                        //{
                        //    continue;
                        //}
                        //foreach (CommandToken token in newVariableTokens)
                        //{
                        //    foreach (List<CommandToken> allTokens in parser.ParsedTokens)
                        //    {
                        //        var index = allTokens.IndexOf(token);
                        //        if (index != -1)
                        //        {
                        //            parser.ParsedTokens[parser.ParsedTokens.IndexOf(allTokens)][index] = token;
                        //        }
                        //    }
                        //}

                        //Run the commands
                        foreach (List<CommandToken> commands in parser.ParsedTokens)
                        {
                            var request = new CommandRequest(commands.ToArray());
                            var routine = new CommandRoutine(request, RunLocalCommand(request, CommandExecutionMode.Client, pipe));
                            pipe.Add(pos++, routine);
                        }
                    }
                    else
                    {
                        if (useProxy)
                        {
                            Proxy.ExecuteProxyCommand(c, CommandExecutionMode.Client);
                        }
                        else
                        {
                            Remote.RunServerCommand(c, CommandExecutionMode.Client);
                        }
                    }
                }
            }
#pragma warning disable CS0162 // Unreachable code detected
            Remote.Close();
#pragma warning restore CS0162 // Unreachable code detected
        }


        //private CommandToken[] RunVariableReplacement(CommandParser p, out bool success)
        //{
        //    success = true;
        //    List<CommandToken> tokenList = new List<CommandToken>();
        //    var variableTokens = p.GetVariables();
        //    foreach (CommandToken variableToken in variableTokens)
        //    {
        //        var variablename = variableToken.OriginalValue.Remove(0, 1);
        //        if (ServerManager.ServerRemoteService.Variables.ContainsKey(variablename))
        //        {
        //            var variableValue = ServerManager.ServerRemoteService.Variables[variablename];
        //            variableToken.Value = variableValue;
        //            success = true;
        //            tokenList.Add(variableToken);
        //        }
        //        else
        //        {
        //            Logger.Log(new UILogItem(LogLevel.Error, $"Variable {variablename} does not exist", "Server Host"));
        //            success = false;
        //        }
        //    }
        //    return tokenList.ToArray();
        //}

        private static IEnumerable<CommandToken> RunSubRoutines(CommandParser p, CommandPipeline pipe, int position)
        {
            foreach (CommandToken routineToken in p.GetSubRoutines())
            {
                var commandToExecute = Regex.Match(routineToken.OriginalValue, CommandToken.SUBROUTINE_PATTERN).Groups[1].Value;
                var parsedCommand = p.Parse(commandToExecute, false);
                foreach (List<CommandToken> allCommands in parsedCommand)
                {
                    var request = new CommandRequest(allCommands.ToArray());
                    var routine = new CommandRoutine(request, RunLocalCommand(request, CommandExecutionMode.Client, pipe));
                    routineToken.Value = routine.Output.CustomStatusMessage;
                    position++;
                }
                yield return routineToken;
            }
        }

        private static void WritePrompt()
        {
            Console.ResetColor();
            if (!string.IsNullOrEmpty(prompt.CurrentUser))
            {
                Console.Write(prompt.CurrentUser);
            }
            if (!string.IsNullOrEmpty(prompt.Path))
            {
                Console.Write(prompt.Path.Insert(0, "$::"));
            }
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(" [");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(prompt.AdditionalData);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("]");
            Console.ResetColor();
            Console.Write(">");
        }

        public static void ShowBanner()
        {
            Random r = new Random();
            Colorful.Console.WriteAscii("REMOTE PLUS!", Color.FromArgb(226, 186, 255));
            var message = "FUN TIP! " + GetRandomTip(r) + "\n";
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        private static string GetRandomTip(Random rand)
        {
            string[] messages =
            {
                "Don't use this software unless you own these systems!",
                "If something doesn't work, don't panic.",
                "When there's a fire, do not use the elevators!",
                "Executing del sys32 /R won't totally kill your system since most of the files are protected.",
                "Don't trust anyone who says deleting System32 will speed up your computers.",
                "Always look everywhere before crossing the street.",
                "If your computer has lots of temp files, use dskClean (which is part of the WIndowsTools extension library) to clean those pesky files.",
                "Never kill a process you don't know about.",
                "The movie, Jaws, is just a horror story about Sharks, people are exaggerating shark attacks to much.",
                "It is more likely for a vending machine to fall on someone then a Shark attack.",
                "It is more likely for you to be struck by lightning then winning big at the casino.",
                "If your hand is higher then 12, don't double down in Black Jack.",
                "There's a secret level in The Legend of Zelda Four Swords Anniversary Edition™ if you get enough Rupees.",
                "Here is a slice of Pi 3.141592653589793238462643383279028841971693993751058209749445923078164062862089986280348253421170679821480865132823066470938446095505822317253594081284811194502841027019385211055596446229489549303819644288109756",
                "The battle in The Princess Pride is really cool but unrealistic.",
                "Lake Superior is the coldest lake in the United States.",
                "Those Youtube vidoes on how to speed up Minecraft work, but you should just get a new computer then wrestling threw all the settings, and besides the game will look ugly at the end.",
                "The Enigma Machine was used during World War 2 for transmitting encrypted information between the Germans.",
                "If you want that cool synthesized voice like the voice in War Games, download NVDA (which is on GitHub)",
                "You can have dark mode on GitHub. The OctoCat would love to show you how to install the plugin.",
                "You can chain commands in RemotePlus by using the & symbole.",
                "Always install AntiVirus software on your computer.",
                "Always keep your AntiVirus software up to date.",
                "Never cook a pizza on a light bulb.",
                "Alexander Hamilton, the director of the treasury during the Presidency of Thomas Jefferson, was slain during a duel with Auron Burr, the vice-President of Thomas Jefferson.",
                "There is a national holiday for each day of the year.",
                "A Doe is a female dear, a Buck is a male dear.",
                "When on a hard problem during a math test, skip that one and go to the next one. It will save you a'lot of time.",
                "If you don't know a word don't say it. You could sound imbecilic.",
                "Do use sesquipedalian words especially people who have HippopotomonstrosesquiPedaliophobia",
                "Fedex has an arrow in its name."
            };
            return messages[rand.Next(messages.Length)];
        }

        static CommandPipeline Input(string i)
        {
            return Remote.RunServerCommand(i, CommandExecutionMode.Client);
        }
        private static void InitCommands()
        {
            LocalCommands.Add("#banner", banner);
            LocalCommands.Add("#help", Help);
            LocalCommands.Add("#clear", clearScreen);
            LocalCommands.Add("#close", close);
            LocalCommands.Add("#title", title);
            LocalCommands.Add("#load-commandFile", load_CommandFile);
            LocalCommands.Add("#execute-script", loadScriptFIle);
        }

        static CommandResponse RunLocalCommand(CommandRequest request, CommandExecutionMode commandMode, CommandPipeline pipe)
        {
            bool throwFlag = false;
            StatusCodeDeliveryMethod scdm = StatusCodeDeliveryMethod.DoNotDeliver;
            try
            {
                bool FoundCommand = false;
                foreach (KeyValuePair<string, CommandDelegate> k in LocalCommands)
                {
                    if (request.Arguments[0].Value == k.Key)
                    {
                        var ba = RemotePlusConsole.GetCommandBehavior(k.Value);
                        if (ba != null)
                        {
                            if (ba.TopChainCommand && pipe.Count > 0)
                            {
                                GlobalServices.Logger.Log($"This is a top-level command.", LogLevel.Error);
                                return new CommandResponse((int)CommandStatus.AccessDenied);
                            }
                            if (commandMode != ba.ExecutionType)
                            {
                                GlobalServices.Logger.Log($"The command requires you to be in {ba.ExecutionType} mode.", LogLevel.Error);
                                return new CommandResponse((int)CommandStatus.AccessDenied);
                            }
                            if (ba.DoNotCatchExceptions)
                            {
                                throwFlag = true;
                            }
                            if (ba.StatusCodeDeliveryMethod != StatusCodeDeliveryMethod.DoNotDeliver)
                            {
                                scdm = StatusCodeDeliveryMethod.TellMessageToServerConsole;
                            }
                        }
                        FoundCommand = true;
                        var sc = k.Value(request, pipe);
                        if (scdm == StatusCodeDeliveryMethod.TellMessage)
                        {
                            GlobalServices.Logger.Log($"Command {k.Key} finished with status code {sc.ToString()}", LogLevel.Info);
                        }
                        else if (scdm == StatusCodeDeliveryMethod.TellMessageToServerConsole)
                        {
                            GlobalServices.Logger.Log($"Command {k.Key} finished with status code {sc.ToString()}", LogLevel.Info);
                        }
                        return sc;
                    }
                }
                if (!FoundCommand)
                {
                    GlobalServices.Logger.Log("Unknown local command. Please type {#help} for a list of commands.", LogLevel.Error);
                    return new CommandResponse((int)CommandStatus.Fail);
                }
                return new CommandResponse(-2);
            }
            catch (Exception ex)
            {
                if (throwFlag)
                {
                    throw;
                }
                else
                {
                    GlobalServices.Logger.Log("Error whie executing local command: " + ex.Message, LogLevel.Error);
                    return new CommandResponse((int)CommandStatus.Fail);
                }
            }
        }
        static void InitializeDefaultKnownTypes()
        {
            GlobalServices.Logger.Log("Initializing default known types.", LogLevel.Info);
            DefaultKnownTypeManager.LoadDefaultTypes();
            DefaultKnownTypeManager.AddType(typeof(UserAccount));
        }
    }
}