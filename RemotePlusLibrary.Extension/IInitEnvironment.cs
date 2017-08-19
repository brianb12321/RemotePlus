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
        bool PreviousError { get; }
    }
}
