using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary;
using Logging;
using System.ServiceModel;
using RemotePlusLibrary.Core;
using System.Windows.Forms;
using System.Drawing;
using RemotePlusClient.CommonUI;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;

namespace RemotePlusClientCmd
{
    public partial class ClientCmdManager
    {
        public static Dictionary<string, CommandDelegate> LocalCommands = new Dictionary<string, CommandDelegate>();
        public static IRemote Remote = null;
        public static CMDLogging Logger = null;
        public static DuplexChannelFactory<IRemote> channel = null;
        public static bool WaitFlag = true;
        [STAThread]
        static void Main(string[] args)
        {
            InitCommands();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Logger = new CMDLogging()
            {
                DefaultFrom = "CLient CMD",
                OverrideLogItemObjectColorValue = true
            };
            InitializeDefaultKnownTypes();
            RequestStore.Init();
            RequestStore.Add("rcmd_menu", new Requests.ConsoleMenuRequest());
            RequestStore.Add("rcmd_smenu", new Requests.SelectableConsoleMenu());
            RequestStore.Add("rcmd_messageBox", new Requests.RCmdMessageBox());
            RequestStore.Add("rcmd_textBox", new Requests.RCmdTextBox());
            if (args.Length == 0)
            {
                try
                {
                    Console.Write("Enter url: ");
                    string url = Console.ReadLine();
                    Console.Write("Enter Username: ");
                    string username = Console.ReadLine();
                    Console.Write("Enter Password: ");
                    string password = Console.ReadLine();
                    RegistirationObject ro = new RegistirationObject();
                    {
                        ro.LoginRightAway = true;
                        ro.Credentials = new UserCredentials(username, password);
                    };
                    Connect(url, ro);
                    AcceptInput();
                }
                catch (Exception ex)
                {
#if DEBUG
                    Logger.AddOutput(new LogItem(OutputLevel.Error, "Client error. " + ex.ToString(), "Client") { Color = ConsoleColor.Red });
#else
                    Logger.AddOutput(new LogItem(OutputLevel.Error, "Client error. " + ex.Message, "Client") { Color = ConsoleColor.Red });
#endif
                }
            }
            else
            {
                try
                {
                    var options = new CommandLineOptions();
                    if (CommandLine.Parser.Default.ParseArguments(args, options))
                    {
                        Connect(options.Url, new RegistirationObject() { LoginRightAway = true, Credentials = new UserCredentials(options.Username, options.Password), VerboseError = options.Verbose });
                        AcceptInput();
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    Logger.AddOutput(new LogItem(OutputLevel.Error, "Client error. " + ex.ToString(), "Client") { Color = ConsoleColor.Red });
#else
                    Logger.AddOutput(new LogItem(OutputLevel.Error, "Client error. " + ex.Message, "Client") { Color = ConsoleColor.Red });
#endif
                }
            }
        }
        static void Connect(string url, RegistirationObject ro)
        {
            channel = new DuplexChannelFactory<IRemote>(new ClientCallback(), "DefaultEndpoint");
            channel.Endpoint.Address = new EndpointAddress(url);
            Remote = channel.CreateChannel();
            Remote.Register(ro);
        }
        static void AcceptInput()
        {
            Console.WriteLine("Enter a command to the server. Type {help} for a list of commands.");
            while (true)
            {
                if (!WaitFlag)
                {
                    Console.Write(">");
                    var c = Console.ReadLine();
                    if (c.ToCharArray()[0] == '#')
                    {
                        var splittedCommand = c.Split('&');
                        int position = 0;
                        foreach (string command in splittedCommand)
                        {
                            CommandPipeline pipeline = new CommandPipeline();
                            CommandRequest request = new CommandRequest(command.Split(' ')
                                .Select(t =>
                                {
                                    // Makes sure that a string that already has a # sign does not have two # signs
                                    if(!t.Contains("#"))
                                    {
                                        return t.Insert(0, "#");
                                    }
                                    else
                                    {
                                        // Leave string alone
                                        return t;
                                    }
                                })
                                .ToArray());
                            var response = RunLocalCommand(request, CommandExecutionMode.Client, pipeline);
                            pipeline.Add(position, new CommandRoutine(request, response));
                            position += 1;
                        }
                    }
                    else
                    {
                        Remote.RunServerCommand(c, CommandExecutionMode.Client);
                    }
                }
            }
#pragma warning disable CS0162 // Unreachable code detected
            channel.Close();
#pragma warning restore CS0162 // Unreachable code detected
        }
        static CommandPipeline Input(string i)
        {
            return Remote.RunServerCommand(i, CommandExecutionMode.Client);
        }
        private static void InitCommands()
        {
            LocalCommands.Add("#help", Help);
            LocalCommands.Add("#clear", clearScreen);
            LocalCommands.Add("#close", close);
            LocalCommands.Add("#title", title);
            LocalCommands.Add("#load-commandFile", load_CommandFile);
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
                    if (request.Arguments[0] == k.Key)
                    {
                        var ba = RemotePlusConsole.GetCommandBehavior(k.Value);
                        if (ba != null)
                        {
                            if (commandMode != ba.ExecutionType)
                            {
                                Logger.AddOutput($"The command requires you to be in {ba.ExecutionType} mode.", OutputLevel.Error);
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
                            Logger.AddOutput($"Command {k.Key} finished with status code {sc.ToString()}", OutputLevel.Info);
                        }
                        else if (scdm == StatusCodeDeliveryMethod.TellMessageToServerConsole)
                        {
                            Logger.AddOutput($"Command {k.Key} finished with status code {sc.ToString()}", OutputLevel.Info);
                        }
                        return sc;
                    }
                }
                if (!FoundCommand)
                {
                    Logger.AddOutput("Unknown local command. Please type {#help} for a list of commands.", OutputLevel.Error);
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
                    Logger.AddOutput("Error whie executing local command: " + ex.Message, OutputLevel.Error);
                    return new CommandResponse((int)CommandStatus.Fail);
                }
            }
        }
        static void InitializeDefaultKnownTypes()
        {
            Logger.AddOutput("Initializing default known types.", OutputLevel.Info);
            DefaultKnownTypeManager.LoadDefaultTypes();
            DefaultKnownTypeManager.AddType(typeof(UserAccount));
        }
    }
}