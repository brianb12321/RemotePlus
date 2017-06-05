using System.Runtime.Serialization;

namespace RemotePlusLibrary
{
    [DataContract]
    public class ReturnData
    {
        [DataMember]
        public object Data { get; private set; }
        public ReturnData(object d)
        {
            Data = d;
        }
    }
}