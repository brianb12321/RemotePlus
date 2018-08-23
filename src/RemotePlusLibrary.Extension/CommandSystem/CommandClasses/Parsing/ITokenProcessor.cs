using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.CommandSystem.CommandClasses.Parsing
{
    /// <summary>
    /// Transforms tokens to their specified type.
    /// </summary>
    public interface ITokenProcessor
    {
        /// <summary>
        /// Attempts to replace the specified token with the value of the specified variable.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="success"></param>
        /// <returns></returns>
        CommandToken[] RunVariableReplacement(IParser p, out bool success);
        /// <summary>
        /// Attempts to replace the specified token with the value of the specified variable.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="success"></param>
        /// <returns></returns>
        CommandToken[] RunVariableReplacement(CommandToken[] tokens, out bool success);
        /// <summary>
        /// Executes a sub command specified in the token and added to the pipeline.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="pipe"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        IEnumerable<CommandToken> RunSubRoutines(IParser p, CommandPipeline pipe, int position);
        /// <summary>
        /// Executes a sub command specified in the token and added to the pipeline.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="pipe"></param>
        /// <param name="position"></param>
        /// <returns></returns>

        IEnumerable<CommandToken> RunSubRoutines(CommandToken[] tokens, IParser p, CommandPipeline pipe, int position);
        IEnumerable<CommandToken> ParseOutQoutes(CommandToken[] tokens);
        IEnumerable<CommandToken> ParseOutQoutes(IParser p);
        void ConfigureProcessor(IDictionary<string, string> variables, ICommandExecutor executor);

    }
}
