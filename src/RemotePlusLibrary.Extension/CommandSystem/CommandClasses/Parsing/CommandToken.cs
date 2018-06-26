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

        public const string SUBROUTINE_PATTERN = @"\$\{([^}]*)\}";
        public const string QOUTE_PATTERN = "\\\"([^\"]*)\"";
        public const string VARIABLE_COMMAND_PATTERN = @"\$\(([^)]*)\)";
        /// <summary>
        /// The type of token.
        /// </summary>
        [DataMember]
        public TokenType Type { get; set; }
        [DataMember]
        public string Value { get; set; }
        [DataMember]
        public string OriginalValue { get; set; }
        private CommandToken(string token)
        {
            OriginalValue = token;
        }
        /// <summary>
        /// Parses the string into the corresponding token.
        /// </summary>
        /// <param name="token">The token to parse.</param>
        /// <returns></returns>
        public static CommandToken Parse(string token)
        {
            CommandToken newToken = null;
            if (Regex.IsMatch(token, SUBROUTINE_PATTERN))
            {
                newToken = new CommandToken(token) {Type = TokenType.SubRoutine };
                return newToken;
            }
            else if (token.StartsWith("$") && token.ToCharArray()[1] != '{')
            {
                newToken = new CommandToken(token) { Type = TokenType.Variable };
                return newToken;
            }
            else if(Regex.IsMatch(token, QOUTE_PATTERN))
            {
                newToken = new CommandToken(token) { Type = TokenType.QouteBody };
                return newToken;
            }
            else if(Regex.IsMatch(token, VARIABLE_COMMAND_PATTERN))
            {
                newToken = new CommandToken(token) { Type = TokenType.VariableCommand };
                return newToken;
            }
            else
            {
                newToken = new CommandToken(token) { Type = TokenType.Command, Value = token };
                return newToken;
            }
        }
        public override string ToString()
        {
            return Value;
        }
    }
}