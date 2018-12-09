using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses.Parsing;

namespace RemotePlusLibrary.Extension.CommandSystem
{
    public class CommandEnvironment : ICommandEnvironmnet
    {
        public ILexer Lexer { get;private set; }

        public ICommandExecutor Executor { get; private set; }
        public ICommandClassStore CommandClasses { get; }
        public IParser Parser { get; }

        public CommandEnvironment(ILexer lexer, ICommandExecutor executor, ICommandClassStore commandClasses, IParser parser)
        {
            Lexer = lexer;
            Parser = parser;
            Executor = executor;
            CommandClasses = commandClasses;
        }
    }
}