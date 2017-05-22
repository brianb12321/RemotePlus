using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension
{
    public abstract class ServerExtension
    {
        public ExtensionDetails GeneralDetails { get; set; }
        protected ServerExtension(ExtensionDetails d)
        {
            GeneralDetails = d;
        }
        public abstract OperationStatus Execute(ExtensionExecutionContext Context, params object[] arguments);
        public abstract void HaultExtension();
        public abstract void ResumeExtension();
    }
}
