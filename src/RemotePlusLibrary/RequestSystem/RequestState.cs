using System.Runtime.Serialization;

namespace RemotePlusLibrary.RequestSystem
{
    [DataContract]
    public enum RequestState
    {
        [EnumMember]
        OK,
        [EnumMember]
        Failed,
        [EnumMember]
        Cancel,
        [EnumMember]
        Exception,
        [EnumMember]
        NotFound
    }
}