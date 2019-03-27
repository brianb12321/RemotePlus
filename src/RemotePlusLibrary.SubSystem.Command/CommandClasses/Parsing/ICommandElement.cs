using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.SubSystem.Command.CommandClasses.Parsing
{
    /// <summary>
    /// Represents command-line data that has been passed, parsed, and evaluated. It could represent a resource query, an ordinary command, or a script.
    /// </summary>
    public interface ICommandElement
    {
        object Value { get; }
        ElementValueType ValueType { get; }
        bool IsOfType<TType>();
        string ToString();
    }
}