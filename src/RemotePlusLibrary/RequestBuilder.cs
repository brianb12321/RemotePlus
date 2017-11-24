using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Windows.Forms;

namespace RemotePlusLibrary
{
    [DataContract]
    public class RequestBuilder
    {
        [DataMember]
        public string Interface { get; private set; }
        [DataMember]
        public string Message { get; private set; }
        [DataMember]
        public AcquisitionMode AcqMode { get; set; } = AcquisitionMode.None;
        [DataMember]
        public Dictionary<string, string> Arguments { get; set; }
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
    }
}