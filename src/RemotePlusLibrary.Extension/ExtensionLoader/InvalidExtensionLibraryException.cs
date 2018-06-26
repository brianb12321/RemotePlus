using System;
using System.Runtime.Serialization;

namespace RemotePlusLibrary.Extension.ExtensionLoader
{
    [Serializable]
    public class InvalidExtensionLibraryException : Exception
    {
        public InvalidExtensionLibraryException()
        {
        }

        public InvalidExtensionLibraryException(string message) : base(message)
        {
        }

        public InvalidExtensionLibraryException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidExtensionLibraryException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}