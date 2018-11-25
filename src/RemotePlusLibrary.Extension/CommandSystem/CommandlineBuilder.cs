using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.CommandSystem
{
    public class CommandlineBuilder
    {
        public IServiceCollection _services;
        public CommandlineBuilder(IServiceCollection services)
        {
            _services = services;
        }
        public CommandlineBuilder UseParser<TParserImpl>() where TParserImpl : IParser
        {
            _services.AddTransient<IParser, TParserImpl>();
            return this;
        }
        public CommandlineBuilder UseProcessor<TProcessorImpl>() where TProcessorImpl : ITokenProcessor
        {
            _services.AddTransient<ITokenProcessor, TProcessorImpl>();
            return this;
        }
        public CommandlineBuilder UseExecutor<TExecuotorImpl>() where TExecuotorImpl : ICommandExecutor
        {
            _services.AddSingleton<ICommandExecutor, TExecuotorImpl>();
            return this;
        }
        public CommandlineBuilder AddCommandClass<TCommandClass>() where TCommandClass : ICommandClass
        {
            _services.AddSingleton<ICommandClass, TCommandClass>();
            return this;
        }
    }
}