using System.Runtime.Serialization;

namespace RemotePlusLibrary.Extension.CommandSystem.CommandClasses.Parsing
{
    [DataContract]
    public enum TokenType
    {
        [EnumMember]
        Variable,
        [EnumMember]
        Command,
        [EnumMember]
        QouteBody,
        [EnumMember]
        SubRoutine,
        [EnumMember]
        VariableCommand
    }
}