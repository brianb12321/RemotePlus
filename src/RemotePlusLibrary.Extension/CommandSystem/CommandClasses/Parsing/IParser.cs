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
    public interface IParser
    {
        List<List<CommandToken>> ParsedTokens { get; set; }
        List<List<CommandToken>> Parse(string command, bool prop);
        List<List<CommandToken>> Parse(bool prop);
        CommandToken[] GetSubRoutines();
        CommandToken[] GetSubRoutines(List<List<CommandToken>> tokens);
        CommandToken[] GetVariables();
        CommandToken[] GetVariables(List<List<CommandToken>> tokens);
        CommandToken[] GetQoutedToken();
        CommandToken[] GetQoutedToken(List<List<CommandToken>> tokens);
    }
}
