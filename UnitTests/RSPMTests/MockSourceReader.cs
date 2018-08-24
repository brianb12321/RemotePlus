using System;
using System.Collections.Generic;
using RSPM;

namespace RSPMTests
{
    internal class MockSourceReader : ISourceReader
    {
        public event EventHandler<SourceEventArgs> ParsedSource;
        string[] dummySources =
        {
            "http://google.com",
            "http://sources.com",
            "http://thisisatest.com"
        };
        public IEnumerable<Uri> ReadSources(string filePath)
        {
            foreach (string testUri in dummySources)
            {
                if (Uri.TryCreate(testUri, UriKind.Absolute, out Uri successUri))
                {
                    yield return successUri;
                }
            }
        }
    }
}