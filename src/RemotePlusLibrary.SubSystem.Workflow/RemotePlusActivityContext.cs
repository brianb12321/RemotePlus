using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.SubSystem.Command;

namespace RemotePlusLibrary.SubSystem.Workflow
{
    public class RemotePlusActivityContext
    {
        public ICommandEnvironment CurrentCommandEnvironment { get; private set; }
        public IServiceCollection ServiceCollection { get; private set; }

        public RemotePlusActivityContext(ICommandEnvironment env, IServiceCollection collection)
        {
            CurrentCommandEnvironment = env;
            ServiceCollection = collection;
        }
    }
}
