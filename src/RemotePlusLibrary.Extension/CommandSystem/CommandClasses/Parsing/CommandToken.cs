using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace RemotePlusLibrary.Extension.CommandSystem.CommandClasses.Parsing
{
    /// <summary>
    /// Represents a token that has been parsed by the parser.
    /// </summary>
    [DataContract]
    public class CommandToken
    {
        [DataMember]
        public string OriginalValue { get; set; }
        [DataMember]
        public TokenType TokenType { get; set; }
        public CommandToken(string orignalValue, TokenType t)
        {
            OriginalValue = orignalValue;
            TokenType = t;
        }
    }
}