using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;

namespace RemotePlusClientCmd
{
    public class CommandLineOptions
    {
        [Option("url", HelpText = "The url used to connect to the server", Required = true)]
        public string Url { get; set; }
        [Option("username", HelpText = "The username that will be sent to the server", Required = true)]
        public string Username { get; set; }
        [Option("password", HelpText = "The password that will be sent to the server", Required = true)]
        public string Password { get; set; }
        [Option('v', "verbose", HelpText = "Tells the server to use verbosity logging", DefaultValue = false)]
        public bool Verbose { get; set; }
        [Option('p', "proxy", HelpText = "When enabled, the client will connect to a proxy server.", DefaultValue = false)]
        public bool UseProxy { get; set; }
        [ParserState]
        public IParserState LastParserState { get; set; }
        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
              (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }

    }
}
