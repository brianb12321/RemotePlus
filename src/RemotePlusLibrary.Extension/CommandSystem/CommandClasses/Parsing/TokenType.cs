using System.Runtime.Serialization;

namespace RemotePlusLibrary.Extension.CommandSystem.CommandClasses.Parsing
{
    [DataContract]
    public enum TokenType
    {
        [EnumMember]
        Resource,
        [EnumMember]
        ExecutionResource,
        [EnumMember]
        CommandElement,
        [EnumMember]
        QouteBody,
        [EnumMember]
        VariableCommand,
        [EnumMember]
        Script,
        [EnumMember]
        Pipe,
        [EnumMember]
        Async,
        [EnumMember]
        FileRedirect,
        [EnumMember]
        AppendFileRedirect,
        [EnumMember]
        ResourceRedirect,
        [EnumMember]
        AppendResourceRedirect,
        [EnumMember]
        InRedirect,
        [EnumMember]
        InResourceRedirect
    }
}