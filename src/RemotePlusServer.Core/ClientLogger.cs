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
            _client.ClientCallback.TellMessage(message, level);
        }
    }
}