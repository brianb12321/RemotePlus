using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary
{
    public class Client<C> where C : IClient
    {
        public string FriendlyName { get; internal set; }
        public C ClientCallback { get; internal set; }
        public Dictionary<string, string> ExtraData { get; internal set; }
        public Guid UniqueID { get; private set; }
        protected Client()
        {
            UniqueID = Guid.NewGuid();
        }
        public static Client<C> Build(ClientBuilder builder, C callback)
        {
            Client<C> c = new Client<C>();
            c.FriendlyName = builder.FriendlyName;
            c.ClientCallback = callback;
            c.ExtraData = builder.ExtraData;
            return c;
        }
    }
}