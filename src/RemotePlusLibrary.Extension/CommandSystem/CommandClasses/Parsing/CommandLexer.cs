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
    public class CommandLexer : ILexer
    {
        public List<CommandToken> Lex(string command)
        {
            List<CommandToken> tokens = new List<CommandToken>();
            for(int i = 0; i < command.Length; i++)
            {
                switch(command[i])
                {
                    case ' ':
                        break;
                    case '>':
                        bool appendMode = false;
                        StringBuilder outputRouteStringBuilder = new StringBuilder();
                        if(command[i + 1] == '>')
                        {
                            appendMode = true;
                        }
                        for(int j = (appendMode) ? i + 3 : i + 2; j <= command.Length; j++)
                        {
                            if(command.Length == j)
                            {
                                if (appendMode == true)
                                {
                                    tokens.Add(new CommandToken(outputRouteStringBuilder.ToString(), TokenType.AppendFileRedirect));
                                    i++;
                                    break;
                                }
                                else
                                {
                                    tokens.Add(new CommandToken(outputRouteStringBuilder.ToString(), TokenType.FileRedirect));
                                    i++;
                                    break;
                                }
                            }
                            else
                            {
                                outputRouteStringBuilder.Append(command[j]);
                                i++;
                            }
                        }
                        break;
                    case '|':
                        StringBuilder pipeBuilder = new StringBuilder();
                        for(int j = i + 2; j <= command.Length; j++)
                        {
                            if(command.Length == j)
                            {
                                List<CommandToken> newTokens = Lex(pipeBuilder.ToString());
                                tokens.Add(new AggregateCommandToken(newTokens, TokenType.Pipe));
                                i++;
                                break;
                            }
                            else
                            {
                                pipeBuilder.Append(command[j]);
                                i++;
                            }
                        }
                        break;
                    case '{':
                        StringBuilder scriptBuilder = new StringBuilder();
                        for (int j = i + 1; j < command.Length; j++)
                        {
                            if (command[j] == '}' && (command.Length == j + 1 || char.IsWhiteSpace(command[j + 1])))
                            {
                                tokens.Add(new CommandToken(scriptBuilder.ToString(), TokenType.Script));
                                i++;
                                break;
                            }
                            else
                            {
                                scriptBuilder.Append(command[j]);
                                i++;
                            }
                        }
                        break;
                    case '$':
                        StringBuilder sb3 = new StringBuilder();
                        for(int j = i + 1; j < command.Length; j++)
                        {
                            if(command[j] != ' ')
                            {
                                sb3.Append(command[j]);
                                i++;
                            }
                            else
                            { 
                                break;
                            }
                        }
                        if(sb3.Length < 2)
                        {
                            tokens.Add(new CommandToken(sb3.ToString(), TokenType.Resource));
                        }
                        else
                        {
                            if (sb3[sb3.Length - 2] == '(' && sb3[sb3.Length - 1] == ')')
                            {
                                sb3.Length -= 2;
                                tokens.Add(new CommandToken(sb3.ToString(), TokenType.ExecutionResource));
                            }
                            else
                            {
                                tokens.Add(new CommandToken(sb3.ToString(), TokenType.Resource));
                            }
                        }
                        break;
                    case '"':
                        bool ignoreQoute = false;
                        bool ignoreBackslash = false;
                        StringBuilder sb = new StringBuilder();
                        for(int j = i + 1; j < command.Length; j++)
                        {
                            if(command[j] == '\\' && ignoreBackslash)
                            {
                                sb.Append(command[j]);
                                ignoreBackslash = false;
                                ignoreQoute = false;
                                i++;
                            }
                            else if(command[j] == '\\')
                            {
                                ignoreQoute = true;
                                ignoreBackslash = true;
                                i++;
                            }
                            else if(command[j] == '"' && ignoreQoute)
                            {
                                sb.Append(command[j]);
                                ignoreQoute = false;
                                ignoreBackslash = false;
                                i++;
                            }
                            else if(command[j] == '"' && !ignoreQoute)
                            {
                                tokens.Add(new CommandToken(sb.ToString(), TokenType.QouteBody));
                                i++;
                                break;
                            }
                            else
                            {
                                sb.Append(command[j]);
                                ignoreQoute = false;
                                ignoreBackslash = false;
                                i++;
                            }
                            
                        }
                        break;
                    default:
                        StringBuilder sb2 = new StringBuilder();
                        for(int j = i; j < command.Length; j++)
                        {
                            if(command[j] != ' ')
                            {
                                sb2.Append(command[j].ToString());
                                i++;
                            }
                            else
                            {
                                break;
                            }
                        }
                        tokens.Add(new CommandToken(sb2.ToString(), TokenType.CommandElement));
                        break;
                }
            }
            return tokens;
        }
    }
}