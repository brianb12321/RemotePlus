using System.Runtime.Serialization;

namespace RemotePlusLibrary.SubSystem.Command.CommandClasses.Parsing
{
    [DataContract]
    public enum TokenType
    {
        [EnumMember]
        CommandName,
        [EnumMember]
        ScriptFile,
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
        DetachIO,
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
        InResourceRedirect,
        [EnumMember]
        SubShellResource
    }
}