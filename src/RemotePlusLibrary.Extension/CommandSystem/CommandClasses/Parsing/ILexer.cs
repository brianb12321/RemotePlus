using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.CommandSystem.CommandClasses.Parsing
{
    /// <summary>
    /// Represents a command parser. A parser a class that splits a command into pieces known as a token.
    /// </summary>
    public interface ILexer
    {
        List<CommandToken> Lex(string command);
    }
}