using System.Runtime.Serialization;

namespace RemotePlusLibrary
{
    [DataContract]
    public enum AutehnticationSeverity
    {
        [EnumMember]
        Risk,
        [EnumMember]
        Normal,
        [EnumMember]
        Danger
    }
}