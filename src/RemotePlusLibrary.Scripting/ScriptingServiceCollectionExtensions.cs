﻿using RemotePlusLibrary.Core.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Scripting
{
    public static class ScriptingServiceCollectionExtensions
    {
        public static IServiceCollection UseScriptingEngine<TImpl>(this IServiceCollection services) where TImpl : IScriptingEngine
        {
            return services.AddSingleton<IScriptingEngine, TImpl>();
        }
    }
}
