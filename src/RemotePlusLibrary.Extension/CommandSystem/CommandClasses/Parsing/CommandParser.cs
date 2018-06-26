using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.CommandSystem.CommandClasses.Parsing
{
    /// <summary>
    /// Parses a string to tokens which can be fed to the command.
    /// </summary>
    public class CommandParser
    {
        public List<List<CommandToken>> ParsedTokens { get; set; } = new List<List<CommandToken>>();
        public string OriginalCommand { get; set; }
        public CommandParser(string command)
        {
            OriginalCommand = command;
        }
        public List<List<CommandToken>> Parse(bool prop)
        {
            return internalParse(OriginalCommand, prop);
        }
        public List<List<CommandToken>> Parse(string command, bool prop)
        {
            return internalParse(command, prop);
        }
        List<List<CommandToken>> internalParse(string command, bool prop)
        {
            //Separates the & from the command to determine how many commands to run in the pipeline.
            string[] commands = command.Split('&');
            //Checks whether to creaste one or more commands.
            if (commands.Length > 1)
            {
                List<List<CommandToken>> tokens = new List<List<CommandToken>>();
                foreach (string singleCommand in commands)
                {
                    var splitCommand = splitCommandBySpace(singleCommand).ToList();
                    if (prop)
                    {
                        ParsedTokens.Add(splitCommand);
                    }
                    tokens.Add(splitCommand);
                }
                //Changes tokens that are variables and subroutines.
                return tokens;
            }
            else
            {
                List<List<CommandToken>> tokens = new List<List<CommandToken>>();
                var splitCommand = splitCommandBySpace(commands[0]).ToList();
                if(prop)
                {
                    ParsedTokens.Add(splitCommand);
                }
                //There is only one command to run.
                // Separate command by space.
                tokens.Add(splitCommand);
                //Changes tokens that are variables and subroutines.
                return tokens;
            }
        }
        CommandToken[] splitCommandBySpace(string command)
        {
            bool appendToken = false;
            List<CommandToken> tokens = new List<CommandToken>();
            // There is only one command to run.
            // Separate command by space.
            string[] spacedString = command.Split(' ');
            string fullToken = "";
            foreach (string token in spacedString)
            {
                #region Check if token begins a subroutine
                if (token.StartsWith("${"))
                {
                    appendToken = true;
                }
                if (token.EndsWith("}"))
                {
                    appendToken = false;
                    fullToken += token;
                }
                #endregion

                #region Check if token is a qouted body
                if(token.StartsWith("\""))
                {
                    appendToken = true;
                }
                if (token.EndsWith("\"") || token.EndsWith("\"\""))
                {
                    appendToken = false;
                    fullToken += token;
                }
                #endregion

                #region Check if append mode is on
                if (appendToken)
                {
                    fullToken += token + " ";
                }
                else
                {
                    if (string.IsNullOrEmpty(fullToken))
                    {
                        tokens.Add(CommandToken.Parse(token));
                    }
                    else
                    {
                        tokens.Add(CommandToken.Parse(fullToken));
                    }
                }
                #endregion
            }
            if(appendToken)
            {
                throw new ParserException("Expected ending qoute.");
            }
            return tokens.ToArray();
        }
        public CommandToken[] GetSubRoutines()
        {
            List<CommandToken> allTokens = new List<CommandToken>();
            foreach(List<CommandToken> tokens in ParsedTokens)
            {
                allTokens.AddRange(tokens.Where(t => t.Type == TokenType.SubRoutine));
            }
            return allTokens.ToArray();
        }
        public CommandToken[] GetSubRoutines(List<List<CommandToken>> tokens)
        {
            List<CommandToken> allTokens = new List<CommandToken>();
            foreach (List<CommandToken> newTokens in tokens)
            {
                allTokens.AddRange(newTokens.Where(t => t.Type == TokenType.SubRoutine));
            }
            return allTokens.ToArray();
        }
        public CommandToken[] GetVariables()
        {
            List<CommandToken> allTokens = new List<CommandToken>();
            foreach (List<CommandToken> tokens in ParsedTokens)
            {
                allTokens.AddRange(tokens.Where(t => t.Type == TokenType.Variable));
            }
            return allTokens.ToArray();
        }
        public CommandToken[] GetVariables(List<List<CommandToken>> tokens)
        {
            List<CommandToken> allTokens = new List<CommandToken>();
            foreach (List<CommandToken> newTokens in tokens)
            {
                allTokens.AddRange(newTokens.Where(t => t.Type == TokenType.Variable));
            }
            return allTokens.ToArray();
        }
        public CommandToken[] GetQoutedToken()
        {
            //CommandToken newToken = null;
            List<CommandToken> allTokens = new List<CommandToken>();
            foreach (List<CommandToken> tokens in ParsedTokens)
            {
                allTokens.AddRange(tokens.Where(t => t.Type == TokenType.QouteBody));
            }
            return allTokens.ToArray();
        }
        public CommandToken[] GetQoutedToken(List<List<CommandToken>> tokens)
        {
            //CommandToken newToken = null;
            List<CommandToken> allTokens = new List<CommandToken>();
            foreach (List<CommandToken> foundTokens in tokens)
            {
                allTokens.AddRange(foundTokens.Where(t => t.Type == TokenType.QouteBody));
            }
            return allTokens.ToArray();
        }
    }
}