using System;
using System.Collections.Generic;

namespace RSPM
{
    public interface ISourceReader
    {
        IEnumerable<Uri> ReadSources(string filePath);
        event EventHandler<SourceEventArgs> ParsedSource;
    }
}