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
                        tokens.Add(new CommandToken(sb3.ToString(), TokenType.Resource));
                        break;
                    case '&':
                        tokens.Add(new CommandToken(command[i].ToString(), TokenType.SubRoutine));
                        break;
                    case '"':
                        StringBuilder sb = new StringBuilder();
                        for(int j = i + 1; j < command.Length; j++)
                        {
                            if(command[j] != '"')
                            {
                                sb.Append(command[j]);
                                i++;
                            }
                            else
                            {
                                tokens.Add(new CommandToken(sb.ToString(), TokenType.QouteBody));
                                i++;
                                break;
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