using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BetterLogger;

namespace NewRemotePlusClient.Models
{
    public class ConsoleLogMessage
    {
        public string Message { get; set; }
        public LogLevel Level { get; set; }
        public string From { get; set; }
        public string Extra { get; set; }
    }
}
