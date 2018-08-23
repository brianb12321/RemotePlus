using RemotePlusLibrary.Extension.CommandSystem.CommandClasses.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.CommandSystem
{
    /// <summary>
    /// Provides the parser, processor, and executor
    /// </summary>
    public interface ICommandEnvironmnet
    {
        IParser Parser { get; }
        ITokenProcessor Processor { get; }
        ICommandExecutor Executor { get; }
    }
}