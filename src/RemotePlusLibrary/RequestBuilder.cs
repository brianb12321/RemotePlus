using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Windows.Forms;

namespace RemotePlusLibrary
{
    /// <summary>
    /// Represents a request for information object.
    /// </summary>
    [DataContract]
    public class RequestBuilder
    {
        /// <summary>
        /// The unique request interface (URI) string used for identifying a request interface to use.
        /// </summary>
        [DataMember]
        public string Interface { get; private set; }
        /// <summary>
        /// The message that will be passed to the URI. This is usually the message explaining why you are requesting information from the client.
        /// </summary>
        [DataMember]
        public string Message { get; private set; }
        /// <summary>
        /// Sets how the client should acquire the data and provides settings for if there was en error during hhe process.
        /// </summary>
        [DataMember]
        public AcquisitionMode AcqMode { get; set; } = AcquisitionMode.None;
        /// <summary>
        /// Proves the data used by the URI to build the RM (request message). Please see your URI documentation for the correct information needed to be passed to the argument
        /// </summary>
        [DataMember]
        public Dictionary<string, string> Arguments { get; set; }
        /// <summary>
        /// Proves extra data and settings used by the URI. These settings are usually used to build the interface. Please see your URI documentation 
        /// </summary>
        [DataMember]
        public Dictionary<string, string> Metadata { get; set; }
        public RequestBuilder(string i, string m, IDictionary<string, string> args)
        {
            Interface = i;
            Message = m;
            Arguments = (Dictionary<string, string>)args;
            Metadata = new Dictionary<string, string>();
        }
        public static RequestBuilder RequestString(string message)
        {
            return new RequestBuilder("r_string", message, null);
        }
        public static RequestBuilder RequestColor()
        {
            return new RequestBuilder("r_color", "", null);
        }
        public static RequestBuilder RequestFilePath(string title)
        {
            return new RequestBuilder("r_filePath", title, null);
        }
        public static RequestBuilder RequestMessageBox(string message, string title, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("Caption", title);
            args.Add("Buttons", buttons.ToString());
            args.Add("Icon", icon.ToString());
            RequestBuilder rb = new RequestBuilder("r_messageBox", message, args);
            return rb;
        }
        public static RequestBuilder RequestFile()
        {
            return new RequestBuilder("global_selectFile", null, null);
        }
    }
}