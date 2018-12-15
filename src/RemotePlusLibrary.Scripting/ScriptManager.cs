using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using RemotePlusLibrary.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Scripting
{
    public class ScriptManager : IScriptManager
    {
        ScriptEngine _engine = null;
        Dictionary<string, ScriptContext> _contexts = new Dictionary<string, ScriptContext>();
        public ScriptManager()
        {
            var paths = _engine.GetSearchPaths();
            paths.Add($"{Environment.CurrentDirectory}\\extensions");
            _engine.SetSearchPaths(paths);
        }

        public void AddScriptContext(string contextName)
        {
            _contexts.Add(contextName, new ScriptContext(_engine));
        }

        public void AddScriptItemToGlobal<TItem>(string name, TItem item)
        {
            var scope = _engine.GetBuiltinModule();
            scope.SetVariable(name, item);
        }

        public void AddScriptType<TType>(TType ns)
        {
            _engine.Execute($"from {typeof(TType).Namespace} import {typeof(TType).Name}", _engine.GetBuiltinModule());
        }

        public void ImportModule(string module)
        {
            _engine.ImportModule(module);
        }

        public TReturn RunString<TReturn>(string scriptContent, string context)
        {
            ScriptSource source = _engine.CreateScriptSourceFromString(scriptContent);
            var result = source.Execute(_contexts[context].GetScope());
            return result;
        }

        public void SetStandordErrorStream(ITextStream stream)
        {
            _engine.Runtime.IO.SetErrorOutput(new MemoryStream(), new ClientTextWriter(stream));
        }

        public void SetStandordInStream(ITextStream stream)
        {
            
        }

        public void SetStandordOutStream(ITextStream stream)
        {
            _engine.Runtime.IO.SetOutput(new MemoryStream(), new ClientTextWriter(stream));
        }
    }
}