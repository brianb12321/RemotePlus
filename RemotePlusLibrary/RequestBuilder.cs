using System.Runtime.Serialization;

namespace RemotePlusLibrary
{
    [DataContract]
    public class RequestBuilder
    {
        [DataMember]
        public RequestType Interface { get; private set; }
        [DataMember]
        public string Message { get; private set; }
        [DataMember]
        public string Constraint { get; private set; }
        public RequestBuilder(RequestType i, string m)
        {
            Interface = i;
            Message = m;
        }
        public static RequestBuilder RequestString(string message)
        {
            return new RequestBuilder(RequestType.String, message);
        }
        public static RequestBuilder RequestColor()
        {
            return new RequestBuilder(RequestType.Color, "");
        }
        public static RequestBuilder RequestFilePath(string title, string constraint)
        {
            return new RequestBuilder(RequestType.FilePath, title)
            {
                Constraint = constraint
            };
        }
    }
}