using System.Runtime.Serialization;

namespace RemotePlusLibrary
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
        Exception
    }
}