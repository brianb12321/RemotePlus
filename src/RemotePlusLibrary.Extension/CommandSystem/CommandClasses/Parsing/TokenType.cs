using System.Runtime.Serialization;

namespace RemotePlusLibrary.Extension.CommandSystem.CommandClasses.Parsing
{
    [DataContract]
    public enum TokenType
    {
        [EnumMember]
        Resource,
        [EnumMember]
        CommandElement,
        [EnumMember]
        QouteBody,
        [EnumMember]
        SubRoutine,
        [EnumMember]
        VariableCommand,
        [EnumMember]
        Script
    }
}