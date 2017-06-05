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

namespace RemotePlusClientCmd
{
    class Program
    {
        public static IRemote Remote = null;
        public static CMDLogging Logger = null;
        public static DuplexChannelFactory<IRemote> channel = null;
        static void Main(string[] args)
        {
            try
            {
                Logger = new CMDLogging();
                Logger.DefaultFrom = "CLient CMD";
                Logger.OverrideLogItemObjectColorValue = true;
                InitializeDefaultKnownTypes();
                Console.Write("Enter url: ");
                string url = Console.ReadLine();
                channel = new DuplexChannelFactory<IRemote>(new ClientCallback(), "DefaultEndpoint");
                channel.Endpoint.Address = new EndpointAddress(url);
                Console.Write("Enter Username: ");
                string username = Console.ReadLine();
                Console.Write("Enter Password: ");
                string password = Console.ReadLine();
                RegistirationObject ro = new RegistirationObject();
                ro.LoginRightAway = true;
                ro.Credentials = new UserCredentials(username, password);
                Remote = channel.CreateChannel();
                Remote.Register(ro);
                Console.WriteLine("Enter a command to the server. Type {help} for a list of commands.");
                while (true)
                {
                    Remote.RunServerCommand(Console.ReadLine());
                }
#pragma warning disable CS0162 // Unreachable code detected
                channel.Close();
#pragma warning restore CS0162 // Unreachable code detected
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
        static void InitializeDefaultKnownTypes()
        {
            Logger.AddOutput("Initializing default known types.", OutputLevel.Info);
            DefaultKnownTypeManager.LoadDefaultTypes();
            DefaultKnownTypeManager.AddType(typeof(UserAccount));
        }
    }
    class ClientCallback : IRemoteClient
    {
        public void Disconnect(string Reason)
        {
            Program.channel.Close();
            Program.Logger.AddOutput(new LogItem(OutputLevel.Error, "Server disconnected. " + Reason, "CLient") { Color = ConsoleColor.Red });
        }

        public ClientBuilder RegisterClient()
        {
            ClientBuilder cb = new ClientBuilder();
            cb.FriendlyName = "RemotePlus Client Command Line";
            cb.ExtraData.Add("ps_appendNewLine", "false");
            return cb;
        }

        public UserCredentials RequestAuthentication(AuthenticationRequest Request)
        {
            Console.WriteLine($"The server requires authentication. Reason: {Request.Reason}");
            Console.Write("Enter Username: ");
            string username = Console.ReadLine();
            Console.Write("Enter Password: ");
            string password = Console.ReadLine();
            return new UserCredentials(username, password);
        }

        public ReturnData RequestInformation(RequestBuilder builder)
        {
            if (builder.Interface == RequestType.Color)
            {
                ColorDialog cd = new ColorDialog();
                if (cd.ShowDialog() == DialogResult.OK)
                {
                    return new ReturnData(cd.Color.ToString());
                }
                else
                {
                    return new ReturnData(Color.Empty.ToString());
                }
            }
            else
            {
                throw new Exception("Invalid request type.");
            }
        }

        public void TellMessage(string Message, OutputLevel o)
        {
            LogItem li = new LogItem(o, Message, "Server Host");
            if (o == OutputLevel.Warning)
            {
                li.Color = Program.Logger.ConsoleForegroundWarning;
            }
            else if (o == OutputLevel.Info)
            {
                li.Color = Program.Logger.ConsoleForegroundInfo;
            }
            else if (o == OutputLevel.Error)
            {
                li.Color = Program.Logger.ConsoleForegroundError;
            }
            else if(o == OutputLevel.Debug)
            {
                li.Color = Program.Logger.ConsoleForegroundDebug;
            }
            Program.Logger.AddOutput(li);
        }

        public void TellMessage(UILogItem li)
        {
            if (Program.Logger.OverrideLogItemObjectColorValue)
            {
                if (li.Level == OutputLevel.Warning)
                {
                    li.Color = Program.Logger.ConsoleForegroundWarning;
                }
                else if(li.Level == OutputLevel.Info)
                {
                    li.Color = Program.Logger.ConsoleForegroundInfo;
                }
                else if (li.Level == OutputLevel.Error)
                {
                    li.Color = Program.Logger.ConsoleForegroundError;
                }
                else if(li.Level == OutputLevel.Debug)
                {
                    li.Color = Program.Logger.ConsoleForegroundDebug;
                }
            }
            Program.Logger.AddOutput(new LogItem(li.Level, li.Message, li.From) { Color = li.Color });
        }

        public void TellMessage(UILogItem[] Logs)
        {
            foreach(UILogItem l in Logs)
            {
                if (Program.Logger.OverrideLogItemObjectColorValue)
                {
                    if (l.Level == OutputLevel.Warning)
                    {
                        l.Color = Program.Logger.ConsoleForegroundWarning;
                    }
                    else if (l.Level == OutputLevel.Info)
                    {
                        l.Color = Program.Logger.ConsoleForegroundInfo;
                    }
                    else if (l.Level == OutputLevel.Error)
                    {
                        l.Color = Program.Logger.ConsoleForegroundError;
                    }
                    else if(l.Level == OutputLevel.Debug)
                    {
                        l.Color = Program.Logger.ConsoleForegroundDebug;
                    }
                }
                Program.Logger.AddOutput(new LogItem(l.Level, l.Message, l.From) { Color = l.Color });
            }
        }

        public void TellMessageToServerConsole(UILogItem li)
        {
            string f = "";
            if (string.IsNullOrEmpty(li.From))
            {
                f = "Server Console " + "Server Host";
            }
            else
            {
                f = "Server Console " + li.From;
            }
            li.From = f;
            if (Program.Logger.OverrideLogItemObjectColorValue)
            {
                if (li.Level == OutputLevel.Warning)
                {
                    li.Color = Program.Logger.ConsoleForegroundWarning;
                }
                else if (li.Level == OutputLevel.Info)
                {
                    li.Color = Program.Logger.ConsoleForegroundInfo;
                }
                else if (li.Level == OutputLevel.Error)
                {
                    li.Color = Program.Logger.ConsoleForegroundError;
                }
                else if(li.Level == OutputLevel.Debug)
                {
                    li.Color = Program.Logger.ConsoleForegroundDebug;
                }
            }
            Program.Logger.AddOutput(new LogItem(li.Level, li.Message, li.From) { Color = li.Color });
        }

        public void TellMessageToServerConsole(string Message)
        {
            Console.WriteLine(Message);
        }
    }
}