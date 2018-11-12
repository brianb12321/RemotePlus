using System.Runtime.Serialization;

namespace RemotePlusLibrary.RequestSystem
{
    [DataContract]
    public class ReturnData : IGenericObject
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

        public TType Resolve<TType>() where TType : class
        {
            return Data as TType;
        }

        public TType UnsafeResolve<TType>() where TType : class
        {
            return (TType)Data;
        }

        public void PutObject<TType>(TType obj) where TType : class
        {
            throw new System.Exception("Return data is immutable.");
        }
    }
}