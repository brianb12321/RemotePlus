using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.CommandSystem.CommandClasses.Parsing
{
    public interface IParser
    {
        List<List<ICommandElement>> Parse(IReadOnlyList<CommandToken> tokens, ICommandEnvironment env);
    }
}