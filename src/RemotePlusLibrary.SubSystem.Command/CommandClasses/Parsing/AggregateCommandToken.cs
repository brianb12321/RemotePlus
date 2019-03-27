using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.SubSystem.Command.CommandClasses.Parsing
{
    class AggregateCommandToken : CommandToken
    {
        public List<CommandToken> Tokens { get; set; }
        public AggregateCommandToken(List<CommandToken> tokens, TokenType t) : base(string.Empty, t)
        {
            Tokens = tokens;
            StringBuilder sb = new StringBuilder();
            foreach(CommandToken token in tokens)
            {
                sb.Append(token.OriginalValue);
                sb.Append(" ");
            }
            OriginalValue = sb.ToString();
        }
    }
}