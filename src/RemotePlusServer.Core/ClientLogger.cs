using BetterLogger;
using RemotePlusLibrary.Client;

namespace RemotePlusServer.Core
{
    public class ClientLogger : ILogger
    {
        Client<RemoteClient> _client = null;
        public ClientLogger(Client<RemoteClient> client)
        {
            _client = client;
        }
        public void Log(string message, LogLevel level)
        {
            string newMessage = message.Remove(0, (message.IndexOf(']') + 3));
            _client.ClientCallback.TellMessage(newMessage, level);
        }
    }
}