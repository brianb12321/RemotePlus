using RemotePlusLibrary.Extension.CommandSystem.CommandClasses.Parsing;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses.Parsing.CommandElements;
using RemotePlusLibrary.Extension.ResourceSystem;
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
        public CommandParser(IResourceManager resM)
        {
            _resourceManager = resM;
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
                    case TokenType.CommandElement:
                        _buffer.Add(new StringCommandElement(tokens[i].OriginalValue));
                        _elements.Add(new StringCommandElement(tokens[i].OriginalValue));
                        break;
                    case TokenType.QouteBody:
                        _buffer.Add(new StringCommandElement(tokens[i].OriginalValue));
                        _elements.Add(new StringCommandElement(tokens[i].OriginalValue));
                        break;
                    case TokenType.SubRoutine:
                        _elements.Add(new AggregateCommandElement(_buffer));
                        _buffer.Clear();
                        break;
                    case TokenType.Resource:
                        _buffer.Add(new ResourceQueryCommandElement(new ResourceQuery(tokens[i].OriginalValue, Guid.Empty)));
                        _elements.Add(new ResourceQueryCommandElement(new ResourceQuery(tokens[i].OriginalValue, Guid.Empty)));
                        break;
                }
            }
            return _elements;
        }
    }
}