using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace Recon
{
    public class MultiTextWriter : TextWriter
    {
        public override Encoding Encoding => Encoding.ASCII;
        IEnumerable<TextWriter> writers;
        public MultiTextWriter(params TextWriter[] w)
        {
            writers = w;
        }
        public MultiTextWriter(IEnumerable<TextWriter> w)
        {
            writers = w;
        }
        public override void Write(string value)
        {
            foreach (var w in writers)
                w.Write(value);
        }

        public override void Flush()
        {
            foreach (var writer in writers)
                writer.Flush();
        }

        public override void Close()
        {
            foreach (var writer in writers)
                writer.Close();
        }

    }
}
