using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.CommandSystem.CommandClasses.Parsing
{
    /// <summary>
    /// Executes a command on the server.
    /// </summary>
    public interface ICommandExecutor
    {
        /// <summary>
        /// Instructs the server to begin execution of a command.
        /// </summary>
        /// <param name="arguments">The tokens that have been processed</param>
        /// <param name="commandMode">Determines the type of the caller</param>
        /// <param name="pipe">The pipeline to append to</param>
        /// <returns></returns>
        CommandResponse Execute(CommandRequest arguments, CommandExecutionMode commandMode, CommandPipeline pipe);
    }
}
