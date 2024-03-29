﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RemotePlusLibrary.SubSystem.Command.CommandClasses.Parsing
{
    /// <summary>
    /// Parses a string to tokens which can be fed to the command.
    /// </summary>
    public class CommandLexer : ILexer
    {

        public List<CommandToken> Lex(string command)
        {
            List<CommandToken> tokens = new List<CommandToken>();
            if(command.Length >= 2 && command.Substring(0, 2) == "./")
            {
                StringBuilder scriptFileBuilder = new StringBuilder();
                for(int i = 2; i < command.Length; i++)
                {
                    if (command[i] != ' ')
                    {
                        scriptFileBuilder.Append(command[i]);
                    }
                    else break;
                }
                tokens.Add(new CommandToken(scriptFileBuilder.ToString(), TokenType.ScriptFile));
                command = command.Substring(scriptFileBuilder.Length + 2);
            }
            for(int i = 0; i < command.Length; i++)
            {
                switch(command[i])
                {
                    case ' ':
                        break;
                    case '&':
                        tokens.Add(new CommandToken("&", TokenType.DetachIO));
                        i++;
                        break;
                    case '>':
                        bool appendMode = false;
                        bool resourceRedirection = false;
                        StringBuilder outputRouteStringBuilder = new StringBuilder();
                        if(command[i + 1] == '>')
                        {
                            appendMode = true;
                        }
                        //Check if resource redirection is enabled.
                        if (command[(appendMode) ? i + 3 : i + 2] == '$')
                        {
                            resourceRedirection = true;
                        }
                        for (int j = (appendMode) ? i + 3 : i + 2; j < command.Length; j++)
                        {
                            if(command[j] != ' ')
                            {
                                outputRouteStringBuilder.Append(command[j]);
                                i++;
                            }
                            else
                            {
                                break;
                            }
                        }
                        if (resourceRedirection)
                        {
                            if (appendMode)
                            {
                                tokens.Add(new CommandToken(outputRouteStringBuilder.ToString(), TokenType.AppendResourceRedirect));
                            }
                            else
                            {
                                tokens.Add(new CommandToken(outputRouteStringBuilder.ToString(), TokenType.ResourceRedirect));
                            }
                            break;
                        }
                        else if (appendMode == true)
                        {
                            tokens.Add(new CommandToken(outputRouteStringBuilder.ToString(), TokenType.AppendFileRedirect));
                        }
                        else
                        {
                            tokens.Add(new CommandToken(outputRouteStringBuilder.ToString(), TokenType.FileRedirect));
                        }
                        break;
                    case '<':
                        bool inputResourceRedirection = false;
                        StringBuilder inputRouteStringBuilder = new StringBuilder();
                        //Check if resource redirection is enabled.
                        if (command[i + 2] == '$')
                        {
                            inputResourceRedirection = true;
                        }
                        for (int j = inputResourceRedirection ? i + 3 : i + 2; j < command.Length; j++)
                        {
                            if (command[j] != ' ')
                            {
                                inputRouteStringBuilder.Append(command[j]);
                                i++;
                            }
                            else
                            {
                                break;
                            }
                        }
                        if (inputResourceRedirection)
                        {
                            tokens.Add(new CommandToken(inputRouteStringBuilder.ToString(), TokenType.InResourceRedirect));
                        }
                        else
                        {
                            tokens.Add(new CommandToken(inputRouteStringBuilder.ToString(), TokenType.InRedirect));
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
                        if (command[i + 1] == '(')
                        {
                            StringBuilder subShellBuilder = new StringBuilder();
                            for (int j = i + 2; j < command.Length; j++)
                            {
                                if(command[j] != ')')
                                {
                                    subShellBuilder.Append(command[j]);
                                    i++;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            tokens.Add(new CommandToken(subShellBuilder.ToString(), TokenType.SubShellResource));
                            i++;
                            break;
                        }
                        for (int j = i + 1; j < command.Length; j++)
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
                    case '\'':
                        bool ignoreSingleQoute = false;
                        bool ignoreBackslash2 = false;
                        StringBuilder singleSb = new StringBuilder();
                        for (int j = i + 1; j < command.Length; j++)
                        {
                            if (command[j] == '\\' && ignoreBackslash2)
                            {
                                singleSb.Append(command[j]);
                                ignoreBackslash2 = false;
                                ignoreSingleQoute = false;
                                i++;
                            }
                            else if (command[j] == '\\')
                            {
                                ignoreSingleQoute = true;
                                ignoreBackslash2 = true;
                                i++;
                            }
                            else if (command[j] == '\'' && ignoreSingleQoute)
                            {
                                singleSb.Append(command[j]);
                                ignoreSingleQoute = false;
                                ignoreBackslash2 = false;
                                i++;
                            }
                            else if (command[j] == '\'' && !ignoreSingleQoute)
                            {
                                tokens.Add(new CommandToken(singleSb.ToString(), TokenType.QouteBody));
                                i++;
                                break;
                            }
                            else
                            {
                                singleSb.Append(command[j]);
                                ignoreSingleQoute = false;
                                ignoreBackslash2 = false;
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
            if(tokens.Count > 0)
            {
                tokens[0].TokenType = (tokens[0].TokenType == TokenType.CommandElement) ? TokenType.CommandName : tokens[0].TokenType;
            }
            return tokens;
        }
    }
}