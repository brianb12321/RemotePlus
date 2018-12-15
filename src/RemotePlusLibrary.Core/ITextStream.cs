using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Core
{
    public interface ITextStream
    {
        void Print(string message);
        void Print(Guid serverGuid, string message);
        void Print(FormattedText text);
        void Print(Guid serverGuid, FormattedText text);
        string Read();
        string ReadLine();
        void PrintLine(string message);
        void PrintLine(Guid serverGuid, string message);
        void PrintLine(FormattedText text);
        void PrintLine(Guid serverGuid, FormattedText message);
    }
}