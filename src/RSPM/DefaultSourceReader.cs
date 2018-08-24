using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSPM
{
    public class DefaultSourceReader : ISourceReader
    {
        public event EventHandler<SourceEventArgs> ParsedSource;

        public IEnumerable<Uri> ReadSources(string filePath)
        {
            StreamReader streamReader = new StreamReader(filePath);
            while(!streamReader.EndOfStream)
            {
                var source = streamReader.ReadLine();
                if(Uri.TryCreate(source, UriKind.Absolute, out Uri parsedUri))
                {
                    ParsedSource?.Invoke(this, new SourceEventArgs(parsedUri));
                    yield return parsedUri;
                }
            }
        }
    }
}