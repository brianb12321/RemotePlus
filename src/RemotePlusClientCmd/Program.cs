using System;
using RemotePlusLibrary;
using System.ServiceModel;
using RemotePlusLibrary.Core;
using System.Windows.Forms;
using System.Drawing;
using RemotePlusLibrary.Security.AccountSystem;
using RemotePlusClient.CommonUI.ConnectionClients;
using BetterLogger;
using RemotePlusClientCmd.ClientExtensionSystem;
using RemotePlusLibrary.Core.IOC;
using RemotePlusClient.CommonUI.Connection;
using RemotePlusLibrary.Core.EventSystem;
using RemotePlusLibrary.RequestSystem;
using RemotePlusClient.CommonUI.Requests;
using RemotePlusLibrary.Discovery.Events;
using RemotePlusLibrary.Extension;
using RemotePlusLibrary.SubSystem.Command;
using RemotePlusLibrary.SubSystem.Command.CommandClasses;
using System.Threading.Tasks;
using RemotePlusClient.CommonUI;
using RemotePlusLibrary.Core.NodeStartup;
using RemotePlusLibrary.Scripting;

namespace RemotePlusClientCmd
{
    public partial class ClientCmdManager : IApplication
    {
        private INodeCoreStartup<IClientBuilder> _core;
        public Guid EnvironmentGuid { get; set; } = Guid.NewGuid();
        public static IExtensionLibraryLoader ExtensionLoader => IOCContainer.GetService<IExtensionLibraryLoader>();
        public static ICommandSubsystem<IClientCmdModule> _commandSubsystem => IOCContainer.GetService<ICommandSubsystem<IClientCmdModule>>();
        public static ServiceClient Remote = null;
        public static PromptBuilder prompt = new PromptBuilder();
        public static ProxyClient Proxy = null;
        private static NotifyIcon applicationIcon => IOCContainer.GetService<NotifyIcon>();
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
            IOCContainer.Provider.AddSingleton<IApplication>(new ClientCmdManager());
            GlobalServices.RunningApplication.Start(args);
        }
        public Task Start(string[] args)
        {
            Console.CursorSize = 100;
            _core = NodeCoreLoader.LoadByScan<IClientBuilder>("RemotePlusClientCmd.DefaultClientCore", NetworkSide.Client);
            _core.AddServices(IOCContainer.Provider);
            ClientInitBuilder builder = new ClientInitBuilder();
            _core.InitializeNode(builder);
            builder.Build().RunTasks();
            IOCContainer.Provider.AddSingleton<Connection>();
            IOCContainer.Provider.AddSingleton<IEventBus, EventBus>();
            EventBus.RemoveEventProxy();
            Console.ResetColor();
            Application.ApplicationExit += (sender, e) => applicationIcon.Dispose();
            ShowBanner();
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
        void Connect(string url, RegisterationObject ro, bool useProxy)
        {
            if (useProxy)
            {
                var ea = new EndpointAddress(url);
                CurrentConnectionData.BaseAddress = ea.Uri.Host;
                CurrentConnectionData.Port = ea.Uri.Port;
                Proxy = new ProxyClient(new ClientCallback(), _ConnectionFactory.BuildBinding(), ea);
                IOCContainer.Provider.AddSingleton(Proxy.ChannelFactory.Endpoint);
                RequestStore.Add(new RemotePlusClient.CommonUI.Requests.SendLocalFileByteStreamRequest(Proxy));
                ClientInitBuilder builder = new ClientInitBuilder();;
                _core.PostInitializeNode(builder);
                builder.Build().RunTasks();
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
                IOCContainer.Provider.AddSingleton(Remote.ChannelFactory.Endpoint);
                RequestStore.Add(new SendLocalFileByteStreamRequest(Remote));
                ClientInitBuilder builder = new ClientInitBuilder(); ;
                _core.PostInitializeNode(builder);
                builder.Build().RunTasks();
                Remote.Register(ro);
                CurrentConnectionData.RemoteConnection = Remote;
            }
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
                        IOCContainer.GetService<ICommandSubsystem<IClientCmdModule>>()
                            .RunServerCommand(c.Substring(1), CommandExecutionMode.Client, null);
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
                "If your computer has lots of temp files, use dskClean (which is part of the WindowsTools extension library) to clean those pesky files.",
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