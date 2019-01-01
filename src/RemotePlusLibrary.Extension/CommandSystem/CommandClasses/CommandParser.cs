using RemotePlusLibrary.Extension.CommandSystem.CommandClasses.Parsing;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses.Parsing.CommandElements;
using RemotePlusLibrary.Extension.ResourceSystem;
using RemotePlusLibrary.Scripting;
using System;
using System.Collections.Generic;
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
        public List<ICommandElement> Parse(IReadOnlyList<CommandToken> tokens)
        {
            List<ICommandElement> _elements = new List<ICommandElement>();
            List<ICommandElement> _buffer = new List<ICommandElement>();
            for(int i = 0; i < tokens.Count; i++)
            {
                var token = tokens[i];
                switch(token.TokenType)
                {
                    case TokenType.Script:
                        object result = _scriptingEngine.ExecuteStringUsingSameScriptScope(token.OriginalValue);
                        var scriptCommandElement = new ScriptCommandElement(result);
                        _buffer.Add(scriptCommandElement);
                        _elements.Add(scriptCommandElement);
                        break;
                    case TokenType.CommandElement:
                        var stringCommandElement = new StringCommandElement(tokens[i].OriginalValue);
                        _buffer.Add(stringCommandElement);
                        _elements.Add(stringCommandElement);
                        break;
                    case TokenType.QouteBody:
                        var qouteBodyCommandElement = new StringCommandElement(tokens[i].OriginalValue);
                        _buffer.Add(qouteBodyCommandElement);
                        _elements.Add(qouteBodyCommandElement);
                        break;
                    case TokenType.SubRoutine:
                        _elements.Add(new AggregateCommandElement(_buffer));
                        _buffer.Clear();
                        break;
                    case TokenType.Resource:
                        var resourceQoueryCommandElement = new ResourceQueryCommandElement(new ResourceQuery(tokens[i].OriginalValue, Guid.Empty));
                        _buffer.Add(resourceQoueryCommandElement);
                        _elements.Add(resourceQoueryCommandElement);
                        break;
                }
            }
            return _elements;
        }
    }
}