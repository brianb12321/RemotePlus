using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary
{
    /// <summary>
    /// Represents a RemotePlusClient. A client is any application that communicates with the server.
    /// </summary>
    /// <typeparam name="C">The type of client-callback to create. This client must implement <see cref="IClient"/></typeparam>
    public class Client<C> : NetNode where C : IClient
    {
        /// <summary>
        /// The basic type of a client. This determines whether the server should treat the client as a command or GUI based client.
        /// </summary>
        public ClientType ClientType { get; private set; }
        /// <summary>
        /// The name of the client that will appear in lists and dialog boxes.
        /// </summary>
        public string FriendlyName { get; internal set; }
        /// <summary>
        /// The functions of client. Use this to execute functions on the client.
        /// </summary>
        public C ClientCallback { get; internal set; }
        /// <summary>
        /// Extra data that the server will consume.
        /// </summary>
        public Dictionary<string, string> ExtraData { get; internal set; }
        /// <summary>
        /// The unique seassion id for each client.
        /// </summary>
        public Guid UniqueID { get; private set; }
        protected Client(ClientType ct)
        {
            ClientType = ct;
            UniqueID = Guid.NewGuid();
        }
        /// <summary>
        /// Creates a new client object. 
        /// </summary>
        /// <param name="builder">The settings used to build the client.</param>
        /// <param name="callback">The callback for the client.</param>
        /// <returns></returns>
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