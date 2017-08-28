using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension
{
    /// <summary>
    /// Provides a way to access the initialization environment. The data only applies during the initialization phase.
    /// </summary>
    public interface IInitEnvironment
    {
        /// <summary>
        /// The position that the current library is sitting.
        /// </summary>
        int InitPosition { get; }
        /// <summary>
        /// Determines whether a previous error occurred. A previous error occures when a previous extension logs an error to the logger.
        /// </summary>
        bool PreviousError { get; }
    }
}
