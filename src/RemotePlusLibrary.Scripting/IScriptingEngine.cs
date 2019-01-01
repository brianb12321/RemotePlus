using Microsoft.Scripting.Hosting;
using System.Collections.Generic;

namespace RemotePlusLibrary.Scripting
{
    public interface IScriptingEngine
    {
        void InitializeGlobals();
        ScriptEngine ScriptingEngine { get; }
        object ExecuteStringUsingSameScriptScope(string script);
        void AddAssembly(string name);
        void AddClass<TClass>();
        void AddScriptObject<T>(string objectName, T scriptObject, string description, ScriptGlobalType objectType) where T : class;
        void ImportModule(string module);
        bool FunctionExists(string functionName);
        List<ScriptGlobal> GetGlobals();
        void InitializeEngine();
        object ExecuteString(string script);
        void ClearStaticScope();
    }
}