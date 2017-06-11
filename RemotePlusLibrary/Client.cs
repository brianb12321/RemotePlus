using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary
{
    public class Client<C> : NetNode where C : IClient
    {
        public ClientType ClientType { get; private set; }
        public string FriendlyName { get; internal set; }
        public C ClientCallback { get; internal set; }
        public Dictionary<string, string> ExtraData { get; internal set; }
        public Guid UniqueID { get; private set; }
        protected Client(ClientType ct)
        {
            ClientType = ct;
            UniqueID = Guid.NewGuid();
        }
        public static Client<C> Build(ClientBuilder builder, C callback)
        {
            Client<C> c = new Client<C>(builder.ClientType)
            {
                FriendlyName = builder.FriendlyName,
                ClientCallback = callback,
                ExtraData = builder.ExtraData
            };
            return c;
        }
    }
}