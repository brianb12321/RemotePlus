using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.AccountSystem
{
    public class RoleException : Exception
    {
        public RoleException() : base()
        {
        }

        public RoleException(string message) : base(message)
        {
        }

        public RoleException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RoleException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
