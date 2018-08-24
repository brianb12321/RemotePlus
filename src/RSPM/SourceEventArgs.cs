using System;

namespace RSPM
{
    public class SourceEventArgs : EventArgs
    {
        public Uri ParsedUri { get; }
        public SourceEventArgs(Uri uri)
        {
            ParsedUri = uri;
        }
    }
}