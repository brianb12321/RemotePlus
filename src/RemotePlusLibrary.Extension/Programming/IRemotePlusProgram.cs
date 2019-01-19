using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses.Parsing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.Programming
{
    /// <summary>
    /// Exposes virtual standord input and output for a RemtotePlus program.
    /// </summary>
    public interface IRemotePlusProgram : IDisposable
    {
        string Name { get; }
        TextWriter Out { get; }
        TextReader In { get; }
        void WriteLine(string text);
        void WriteLine(string text, Color color);
        void WriteLine(ConsoleText text);
        void WriteLine();
        void Write(string text);
        void Write(ConsoleText text);
        void SetOut(TextWriter writer);
        void SetIn(TextReader reader);
        CommandResponse Execute(CommandRequest req, CommandExecutionMode mode);
    }
}