using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses.Parsing;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses.Parsing.CommandElements;
using RemotePlusLibrary.Extension.ResourceSystem;
using RemotePlusLibrary.Extension.ResourceSystem.ResourceTypes;
using RemotePlusLibrary.Scripting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.CommandSystem.CommandClasses
{
    public class CommandParser : IParser
    {
        IResourceManager _resourceManager;
        IScriptingEngine _scriptingEngine;
        public CommandParser(IResourceManager resM, IScriptingEngine scriptingEngine)
        {
            _resourceManager = resM;
            _scriptingEngine = scriptingEngine;
        }
        public List<List<ICommandElement>> Parse(IReadOnlyList<CommandToken> tokens, ICommandEnvironment env)
        {
            List<List<ICommandElement>> _finalList = new List<List<ICommandElement>>();
            List<ICommandElement> _elements = new List<ICommandElement>();
            for(int i = 0; i < tokens.Count; i++)
            {
                var token = tokens[i];
                switch(token.TokenType)
                {
                    case TokenType.Script:
                        var scriptCommandElement = new ScriptCommandElement(token.OriginalValue);
                        _elements.Add(scriptCommandElement);
                        break;
                    case TokenType.CommandElement:
                        var stringCommandElement = new StringCommandElement(tokens[i].OriginalValue);
                        _elements.Add(stringCommandElement);
                        break;
                    case TokenType.QouteBody:
                        var qouteBodyCommandElement = new StringCommandElement(tokens[i].OriginalValue);
                        _elements.Add(qouteBodyCommandElement);
                        break;
                    case TokenType.Pipe:
                        _finalList.Add(_elements);
                        _elements = new List<ICommandElement>();
                        AggregateCommandToken aggregate = tokens[i] as AggregateCommandToken;
                        List<List<ICommandElement>> newElements = Parse(aggregate.Tokens.AsReadOnly(), env);
                        _finalList.AddRange(newElements);
                        break;
                    case TokenType.FileRedirect:
                        StreamWriter sw = new StreamWriter(tokens[i].OriginalValue, false);
                        env.SetOut(sw);
                        break;
                    case TokenType.AppendFileRedirect:
                        StreamWriter swAppend = new StreamWriter(tokens[i].OriginalValue, true);
                        env.SetOut(swAppend);
                        break;
                    case TokenType.ResourceRedirect:
                        IOResource resource = _resourceManager.GetResource<IOResource>(new ResourceQuery(tokens[i].OriginalValue.Substring(1), Guid.Empty));
                        StreamWriter tw = new StreamWriter(resource.OpenWriteStream());
                        env.SetOut(tw);
                        break;
                    case TokenType.AppendResourceRedirect:
                        IOResource appendResource = _resourceManager.GetResource<IOResource>(new ResourceQuery(tokens[i].OriginalValue.Substring(1), Guid.Empty));
                        StreamWriter atw = new StreamWriter(appendResource.OpenWriteStream());
                        atw.BaseStream.Seek(0, SeekOrigin.End);
                        env.SetOut(atw);
                        break;
                    case TokenType.ExecutionResource:
                        var executionResourceQueryCommandElement = new ExecutionResourceQueryCommandElement(new ResourceQuery(tokens[i].OriginalValue, Guid.Empty));
                        _elements.Add(executionResourceQueryCommandElement);
                        break;
                    case TokenType.Resource:
                        var resourceQoueryCommandElement = new ResourceQueryCommandElement(new ResourceQuery(tokens[i].OriginalValue, Guid.Empty));
                        _elements.Add(resourceQoueryCommandElement);
                        break;
                }
            }
            //Add all command elements in this code if there are no pipe symbols.
            if (_finalList.Count == 0)
            {
                _finalList.Add(_elements);
            }
            return _finalList;
        }
    }
}