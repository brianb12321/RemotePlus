using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RemotePlusLibrary;
using System.ServiceModel;
using RemotePlusLibrary.Core;

namespace RemotePlusServerCmd
{
    class Program : IRemoteClient
    {
        public static IRemote Remote;
        public static LogWrapper Logger;
        public static LogWrapper ServerLogger;
        public static DuplexChannelFactory<IRemote> channel = null;
        static bool readyFlag = false;
        static void Main(string[] args)
        {
            try
            {
                Logger = new LogWrapper("Client");
                ServerLogger = new LogWrapper("Server Host");
                Console.Write("Enter connection url: ");
                string url = Console.ReadLine();
                channel = new DuplexChannelFactory<IRemote>(new Program(), new NetTcpBinding(), new EndpointAddress(url));
                Remote = channel.CreateChannel();
                Console.WriteLine("Pre authentication.");
                Console.Write("Enter Username: ");
                string username = Console.ReadLine();
                Console.Write("Enter Password: ");
                string password = Console.ReadLine();              
                RegistirationObject robj = new RegistirationObject();
                robj.LoginRightAway = true;
                robj.Credentials = new UserCredentials(username, password);
                Remote.Register(robj);
                readyFlag = true;
                if (readyFlag)
                {
                    Console.WriteLine("RemotePlus client. Type {help} for a list of commands.");
                    while (true)
                    {
                        string command = Console.ReadLine();
                        Remote.RunServerCommand(command);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Fatal("Client error: " + ex.Message);
            }
        }

        public void Disconnect(string Reason)
        {
            Logger.Error("The server disconnected. Reason: " + Reason);
            channel.Close();
        }

        public ClientBuilder RegisterClient()
        {
            ClientBuilder cb = new ClientBuilder();
            cb.FriendlyName = "RemotePlusClientCmd";
            return cb;
        }

        public UserCredentials RequestAuthentication()
        {
            Console.WriteLine("The Server Requires authentication.");
            Console.Write("Enter Username: ");
            string username = Console.ReadLine();
            Console.Write("Enter Password: ");
            string password = Console.ReadLine();
            readyFlag = true;
            return new UserCredentials(username, password);
        }

        public void TellMessageError(string Message)
        {
            ServerLogger.Error(Message);
        }

        public void TellMessageFatal(string Message)
        {
            ServerLogger.Fatal(Message);
        }

        public void TellMessageInfo(string Message)
        {
            ServerLogger.Info(Message);
        }

        public void TellMessageToServerConsoleError(string Message, string From)
        {
            ServerLogger.Error($"{From}: {Message}");
        }

        public void TellMessageToServerConsoleFatal(string Message, string From)
        {
            ServerLogger.Fatal($"{From}: {Message}");
        }

        public void TellMessageToServerConsoleInfo(string Message, string From)
        {
            ServerLogger.Info($"{From}: {Message}");
        }

        public void TellMessageToServerConsoleWarning(string Message, string From)
        {
            ServerLogger.Warn($"{From}: {Message}");
        }

        public void TellMessageWarning(string Message)
        {
            ServerLogger.Warn(Message);
        }
    }
}
