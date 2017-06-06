using System.Runtime.Serialization;

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
        public string Constraint { get; private set; }
        [DataMember]
        public AcquisitionMode AcqMode { get; set; } = AcquisitionMode.None;
        public RequestBuilder(string i, string m)
        {
            Interface = i;
            Message = m;
        }
        public static RequestBuilder RequestString(string message)
        {
            return new RequestBuilder("r_string", message);
        }
        public static RequestBuilder RequestColor()
        {
            return new RequestBuilder("r_color", "");
        }
        public static RequestBuilder RequestFilePath(string title, string constraint)
        {
            return new RequestBuilder("r_filePath", title)
            {
                Constraint = constraint
            };
        }
    }
}