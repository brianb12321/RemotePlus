using System.Runtime.Serialization;

namespace RemotePlusLibrary
{
    [DataContract]
    public class ReturnData
    {
        [DataMember]
        public object Data { get; private set; }
        [DataMember]
        public RequestState AcquisitionState { get; private set; }
        public ReturnData(object d, RequestState ac)
        {
            Data = d;
            AcquisitionState = ac;
        }
    }
}