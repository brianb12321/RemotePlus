using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RemotePlusLibrary.Core
{
    public class ClientTextWriter : TextWriter
    {
        ITextStream _stream = null;
        public ClientTextWriter(ITextStream stream)
        {
            _stream = stream;
        }
        public override void Write(char value)
        {
            _stream.Print(value.ToString());
        }

        public override void Write(string value)
        {
            _stream.Print(value);
        }

        public override Encoding Encoding
        {
            get { return Encoding.ASCII; }
        }
    }
}
