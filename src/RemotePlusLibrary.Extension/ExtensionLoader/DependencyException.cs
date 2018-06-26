using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.ExtensionLoader
{
    [Serializable]
    public class DependencyException : Exception
    {
        public DependencyException()
        {
        }

        public DependencyException(string message) : base(message)
        {
        }

        public DependencyException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DependencyException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
