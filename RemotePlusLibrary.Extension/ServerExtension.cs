using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Extension.Programmer;

namespace RemotePlusLibrary.Extension
{
    public abstract class ServerExtension : IExtension<ExtensionDetails>
    {
        public virtual void ProgramRequested(ServerExtensionProgrammerUpdateEvent requestProgrammer)
        {

        }
        protected ServerExtension(ExtensionDetails d)
        {
            GeneralDetails = d;
        }

        public ExtensionDetails GeneralDetails { get; private set; }

        public abstract OperationStatus Execute(ExtensionExecutionContext Context, params object[] arguments);
        public abstract void HaultExtension();
        public abstract void ResumeExtension();
    }
}
