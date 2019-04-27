using RemotePlusLibrary.Core.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Core;

namespace RemotePlusLibrary.Scripting
{
    public static class ScriptingServiceCollectionExtensions
    {
        public static IServiceCollection UseScriptingEngine<TImpl>(this IServiceCollection services)
            where TImpl : IScriptingEngine
        {
            services.AddSingleton<IScriptingEngine, TImpl>();
            services.AddTransient<IClientContextExtensionProvider, ScriptingEngineExtensionProvider>();
            return services;
        }
    }
}