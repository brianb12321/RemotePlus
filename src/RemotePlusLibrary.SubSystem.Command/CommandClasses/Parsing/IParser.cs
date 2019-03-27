using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.SubSystem.Command.CommandClasses.Parsing
{
    public interface IParser
    {
        (CommandEnvironmentOptions Options, List<List<ICommandElement>> Elements) Parse(IReadOnlyList<CommandToken> tokens, ICommandEnvironment env);
    }
}