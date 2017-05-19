using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary;
using Logging;
using System.ServiceModel;

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
                Logger.OverrideLogItemObjectColorValue = true;
                Console.Write("Enter url: ");
                string url = Console.ReadLine();
                channel = new DuplexChannelFactory<IRemote>(new ClientCallback(), new NetTcpBinding(), url);
                Console.Write("Enter Username: ");
                string username = Console.ReadLine();
                Console.Write("Enter Password: ");
                string password = Console.ReadLine();
                RegistirationObject ro = new RegistirationObject();
                ro.LoginRightAway = true;
                ro.Credentials = new UserCredentials(username, password);
                Remote = channel.CreateChannel();
                Remote.Register(ro);
                while (true)
                {
                    Console.WriteLine("Enter a command to the server. Type {help} for a list of commands.");
                    Remote.RunServerCommand(Console.ReadLine());
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Logger.AddOutput(new LogItem(OutputLevel.Exception, "Client error. " + ex.ToString(), "Client"));
#else
                Logger.AddOutput(new LogItem(OutputLevel.Exception, "Client error. " + ex.Message, "Client"));
#endif
            }
        }
    }
    class ClientCallback : IRemoteClient
    {
        public void Disconnect(string Reason)
        {
            Program.Logger.AddOutput(new LogItem(OutputLevel.Error, "Server disconnected. " + Reason, "CLient"));
        }

        public ClientBuilder RegisterClient()
        {
            ClientBuilder cb = new ClientBuilder();
            cb.FriendlyName = "RemotePlus Client Command Line";
            return cb;
        }

        public UserCredentials RequestAuthentication()
        {
            Console.WriteLine("The server requires authentication.");
            Console.Write("Enter Username: ");
            string username = Console.ReadLine();
            Console.Write("Enter Password: ");
            string password = Console.ReadLine();
            return new UserCredentials(username, password);
        }

        public void TellMessage(string Message, OutputLevel o)
        {
            Program.Logger.AddOutput(new LogItem(o, Message, "Server Host"));
        }

        public void TellMessage(UILogItem li)
        {
            Program.Logger.AddOutput(new LogItem(li.Level, li.Message, li.From));
        }

        public void TellMessage(UILogItem[] Logs)
        {
            foreach(UILogItem l in Logs)
            {
                Program.Logger.AddOutput(new LogItem(l.Level, l.Message, l.From));
            }
        }

        public void TellMessageToServerConsole(UILogItem li)
        {
            li.From = "Server Console";
            Program.Logger.AddOutput(new LogItem(li.Level, li.Message, li.From));
        }

        public void TellMessageToServerConsole(string Message)
        {
            Console.WriteLine(Message);
        }
    }
}
