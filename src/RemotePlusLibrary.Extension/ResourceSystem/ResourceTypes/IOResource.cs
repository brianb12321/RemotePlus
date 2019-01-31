using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.ResourceSystem.ResourceTypes
{
    [DataContract]
    [Serializable]
    public abstract class IOResource : Resource, IDisposable
    {
        protected IOResource(string id) : base(id)
        {
        }
        public abstract Stream OpenReadStream();
        public abstract Stream OpenWriteStream();
        public abstract void Write(byte[] data, int offset, int length);
        public abstract int Read(byte[] buffer, int offset, int length);
        public abstract void Close();

        public void Dispose()
        {
            Close();
        }
    }
}