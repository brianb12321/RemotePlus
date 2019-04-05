using System;
using System.Collections.Generic;
using RemotePlusLibrary;
using System.ServiceModel;
using RemotePlusLibrary.Core;
using System.Windows.Forms;
using System.Drawing;
using System.Text.RegularExpressions;
using RemotePlusLibrary.Security.AccountSystem;
using RemotePlusClient.CommonUI.ConnectionClients;
using RemotePlusLibrary.Core.Faults;
using BetterLogger;
using BetterLogger.Loggers;
using System.IO;
using RemotePlusClientCmd.ClientExtensionSystem;
using RemotePlusLibrary.Core.IOC;
using RemotePlusClient.CommonUI.Connection;
using RemotePlusLibrary.Core.EventSystem;
using RemotePlusLibrary.RequestSystem;
using RemotePlusClient.CommonUI.Requests;
using RemotePlusLibrary.Discovery.Events;
using System.Text;
using System.Linq;
using RemotePlusLibrary.Extension;
using System.Reflection;
using RemotePlusLibrary.SubSystem.Command;
using RemotePlusLibrary.SubSystem.Command.CommandClasses.Parsing;
using RemotePlusLibrary.SubSystem.Command.CommandClasses;
using System.Threading.Tasks;
using System.Threading;

namespace RemotePlusClientCmd
{
    public partial class ClientCmdManager : IEnvironment
    {
        public Dictionary<string, CommandDelegate> LocalCommands = new Dictionary<string, CommandDelegate>();
        public static IExtensionLibraryLoader ExtensionLoader { get; set; }
        public static ICommandSubsystem<IClientCmdModule> _commandSubsystem;
        public static ServiceClient Remote = null;
        public static PromptBuilder prompt = new PromptBuilder();
        public static ProxyClient Proxy = null;
        private static NotifyIcon applicationIcon;
        public Connection CurrentConnectionData => IOCContainer.GetService<Connection>();
        public IEventBus EventBus => IOCContainer.GetService<IEventBus>();
        public static bool WaitFlag = true;
        public int _cursorStop;
        public bool ProxyEnabled { get; private set; }
        public NetworkSide ExecutingSide => NetworkSide.Client;

        public EnvironmentState State { get; private set; } = EnvironmentState.Created;

        [STAThread]
        static void Main(string[] args)
        {
            IOCContainer.Provider.Bind<IEnvironment>().ToConstant(new ClientCmdManager());
            GlobalServices.RunningEnvironment.Start(args);
        }
        public Task Start(string[] args)
        {
            Console.CursorSize = 100;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            var blf = new BaseLogFactory();
            blf.AddLogger(new ConsoleLogger());
            IOCContainer.Provider.Bind<ILogFactory>().ToConstant(blf);
            ExtensionLoader = new DefaultExtensionLoader(blf);
            _commandSubsystem = new ClientCmdExtensionSubsystem(ExtensionLoader);
            IOCContainer.Provider.Bind<IExtensionLibraryLoader>().ToConstant(ExtensionLoader);
            IOCContainer.Provider.Bind<ICommandSubsystem<IClientCmdModule>>().ToConstant(_commandSubsystem);
            applicationIcon = new NotifyIcon();
            applicationIcon.Icon = SystemIcons.Application;
            applicationIcon.Visible = true;
            IOCContainer.Provider.Bind<Connection>().ToSelf().InSingletonScope();
            IOCContainer.Provider.Bind<RemotePlusLibrary.Configuration.IConfigurationDataAccess>().To(typeof(RemotePlusLibrary.Configuration.StandordDataAccess.ConfigurationHelper)).InSingletonScope().Named("DefaultConfigDataAccess");
            IOCContainer.Provider.Bind<IEventBus>().To(typeof(EventBus)).InSingletonScope();
            EventBus.RemoveEventProxy();
            Console.ResetColor();
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
                if (e.IsTerminating)
                {
                    Environment.Exit(-1);
                    applicationIcon.Dispose();
                }
            };
            Application.ApplicationExit += (sender, e) => applicationIcon.Dispose();
            ShowBanner();
            ExtensionLoader.LoadFromAssembly(Assembly.GetAssembly(typeof(ClientCmdCommands)));
            ExtensionLoader.LoadFromFolder("extensions");
            _commandSubsystem.Init();
            RequestStore.Add(new RequestStringRequest());
            RequestStore.Add(new ColorRequest());
            RequestStore.Add(new MessageBoxRequest());
            RequestStore.Add(new SelectLocalFileRequest());
            RequestStore.Add(new SelectFileRequest());
            RequestStore.Add(new SendFilePackageRequest());
            RequestStore.Add(new Requests.ConsoleMenuRequest());
            RequestStore.Add(new Requests.SelectableConsoleMenu());
            RequestStore.Add(new Requests.RCmdMessageBox());
            RequestStore.Add(new Requests.RCmdTextBox());
            RequestStore.Add(new Requests.RCmdMultiLineTextbox());
            RequestStore.Add(new Requests.ConsoleProgressRequest());
            RequestStore.Add(new Requests.ConsoleReadLineRequest());
            InitializeDefaultKnownTypes();
            GlobalServices.Logger.Log("Running post init on all extensions.", LogLevel.Info);
            ExtensionLoader.RunPostInit();
            State = EnvironmentState.Running;
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
            return Task.CompletedTask;
        }

        void CatchException(Exception ex)
        {
            using (ErrorDialog d = new ErrorDialog(ex))
            {
                d.ShowDialog();
            }
        }
        void Connect(string url, RegisterationObject ro, bool useProxy)
        {
            if (useProxy)
            {
                var ea = new EndpointAddress(url);
                CurrentConnectionData.BaseAddress = ea.Uri.Host;
                CurrentConnectionData.Port = ea.Uri.Port;
                Proxy = new ProxyClient(new ClientCallback(), _ConnectionFactory.BuildBinding(), ea);
                RequestStore.Add(new RemotePlusClient.CommonUI.Requests.SendLocalFileByteStreamRequest(Proxy));
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
                CurrentConnectionData.RemoteConnection = Proxy;
                EventBus.Subscribe<ServerAddedEvent>(e =>
                {
                    applicationIcon.ShowBalloonTip(2000, "Server Joined", $"{e.ServerGuid} joined the proxy server.", ToolTipIcon.Info);
                });
                EventBus.Subscribe<ServerDisconnectedEvent>(e =>
                {
                    if(e.Faulted)
                    {
                        applicationIcon.ShowBalloonTip(2000, "Server Disconnected", $"{e.ServerGuid} disconnected suddenly.", ToolTipIcon.Error);
                    }
                    else
                    {
                        applicationIcon.ShowBalloonTip(2000, "Server Disconnected", $"{e.ServerGuid} disconnected gracefully.", ToolTipIcon.Info);
                    }
                });
                EventBus.Subscribe<ServerMessageEvent>(e =>
                {
                    applicationIcon.ShowBalloonTip(1000, "Server Message", $"{e.ServerGuid}: {e.Message}", ToolTipIcon.Info);
                });
            }
            else
            {
                var ea = new EndpointAddress(url);
                CurrentConnectionData.BaseAddress = ea.Uri.Host;
                CurrentConnectionData.Port = ea.Uri.Port;
                Remote = new ServiceClient(new ClientCallback(), _ConnectionFactory.BuildBinding(), ea);
                RequestStore.Add(new SendLocalFileByteStreamRequest(Remote));
                Remote.Register(ro);
                CurrentConnectionData.RemoteConnection = Remote;
            }
            //Console.TreatControlCAsInput = true;
            Console.CancelKeyPress += Console_CancelKeyPress;
        }

        private void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            if (Proxy != null) Proxy.CancelProxyCommand();
            else Remote.CancelServerCommand();
            e.Cancel = true;
        }

        void AcceptInput(bool useProxy)
        {
            Console.WriteLine("Enter a command to the server. Type {help} for a list of commands.");
            while (true)
            {
                if (!WaitFlag)
                {
                    WritePrompt();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    string c = ConsoleHelper.LongReadLine();
                    Console.ResetColor();
                    if (string.IsNullOrEmpty(c))
                    {
                        c = " ";
                    }
                    if (c.ToCharArray()[0] == '#')
                    {
                        CommandPipeline pipe = new CommandPipeline();
                        CommandLexer lexer = new CommandLexer();
                        CommandParser parser = new CommandParser(null, null);
                        var tokens = lexer.Lex(c);
                        var (options, elements) = parser.Parse(tokens, null);
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
                        if(elements.Count <= 1)
                        {
                            var request = new CommandRequest(elements[0].ToArray(), CancellationToken.None);
                            var routine = new CommandRoutine(request, RunLocalCommand(request, CommandExecutionMode.Client, pipe));
                            pipe.Add(routine);
                        }
                        else
                        {
                            CommandResponse result = null;
                            foreach (List<ICommandElement> currentCommand in elements)
                            {
                                if (elements.IndexOf(currentCommand) == 0)
                                {
                                    CommandRequest firstRequest = new CommandRequest(currentCommand.ToArray(), CancellationToken.None);
                                    var firstRoutine = new CommandRoutine(firstRequest, RunLocalCommand(firstRequest, CommandExecutionMode.Client, pipe));
                                    result = firstRoutine.Output;
                                    pipe.Add(firstRoutine);
                                }
                                else
                                {
                                    CommandRequest request = new CommandRequest(currentCommand.ToArray(), CancellationToken.None);
                                    request.LastCommand = result;
                                    var routine = new CommandRoutine(request, RunLocalCommand(request, CommandExecutionMode.Client, pipe));
                                    result = routine.Output;
                                    pipe.Add(routine);
                                }
                            }
                        }
                    }
                    else if(c.ToCharArray()[0] == '&')
                    {
                        if (useProxy)
                        {
                            Proxy.ExecuteProxyCommandAsync(c.Substring(1), CommandExecutionMode.Client);
                        }
                        else
                        {
                            Remote.RunServerCommandAsync(c.Substring(1), CommandExecutionMode.Client);
                        }
                    }
                    else
                    {
                        if (useProxy)
                        {
                            Proxy.ExecuteProxyCommandAsync(c, CommandExecutionMode.Client).Wait();
                        }
                        else
                        {
                            Remote.RunServerCommandAsync(c, CommandExecutionMode.Client);
                        }
                    }
                }
            }
#pragma warning disable CS0162 // Unreachable code detected
            Remote.Close();
#pragma warning restore CS0162 // Unreachable code detected
        }

        
        private void WritePrompt()
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

        CommandPipeline Input(string i)
        {
            return Remote.RunServerCommand(i, CommandExecutionMode.Client);
        }

        CommandResponse RunLocalCommand(CommandRequest request, CommandExecutionMode commandMode, CommandPipeline pipe)
        {
            bool throwFlag = false;
            StatusCodeDeliveryMethod scdm = StatusCodeDeliveryMethod.DoNotDeliver;
            try
            {
                if(!_commandSubsystem.HasCommand(request.Arguments[0].ToString()))
                {
                    GlobalServices.Logger.Log("Unknown local command. Please type {#help} for a list of commands.", LogLevel.Error);
                    return new CommandResponse((int)CommandStatus.Fail);
                }
                var command = _commandSubsystem.GetCommand(request.Arguments[0].ToString());
                var ba = _commandSubsystem.GetCommandBehavior(command);
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
                var sc = command(request, pipe, null);
                if (scdm == StatusCodeDeliveryMethod.TellMessage)
                {
                    GlobalServices.Logger.Log($"Command {request.Arguments[0]} finished with status code {sc.ToString()}", LogLevel.Info);
                }
                else if (scdm == StatusCodeDeliveryMethod.TellMessageToServerConsole)
                {
                    GlobalServices.Logger.Log($"Command {request.Arguments[0]} finished with status code {sc.ToString()}", LogLevel.Info);
                }
                return sc;
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
        void InitializeDefaultKnownTypes()
        {
            GlobalServerBuilderExtensions.InitializeKnownTypes();
        }

        public void Close()
        {
            State = EnvironmentState.Closing;
            Remote?.Disconnect();
            Remote?.Close();
            Proxy?.ProxyDisconnect();
            Proxy?.Close();
            Environment.Exit(0);
        }
    }
}