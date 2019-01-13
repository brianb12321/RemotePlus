using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusClientCmd
{
    public static class ConsoleHelper
    {
        public static string LongReadLine()
        {
            const int BUFFER_SIZE = sizeof(long);
            Stream stream = Console.OpenStandardInput(BUFFER_SIZE);
            StreamReader sr = new StreamReader(stream);
            return sr.ReadLine();
        }
    }
}