using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using IronPython;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using RemotePlusLibrary.Core;

namespace RemotePlusLibrary.Scripting
{
    public class IronPythonScriptingEngine : IScriptingEngine
    {
        private Dictionary<string, IScriptExecutionContext> _storedContexts = new Dictionary<string, IScriptExecutionContext>();
        public ScriptEngine ScriptingEngine { get; private set; }
        public IronPythonScriptingEngine()
        {
            ScriptingEngine = Python.CreateEngine();
            AddPath(AppDomain.CurrentDomain.BaseDirectory + "\\PythonStdLib");
        }

        public void AddPath(string path)
        {
            List<string> paths = ScriptingEngine.GetSearchPaths().ToList();
            paths.Add(path);
            ScriptingEngine.SetSearchPaths(paths);
        }

        public IScriptExecutionContext ExecuteFile(string path)
        {
            return ExecuteFile(path, new IronPythonScriptExecutionContext(ScriptingEngine));
        }

        public IScriptExecutionContext ExecuteFile(string path, IScriptExecutionContext context)
        {
            IronPythonScriptExecutionContext ironPythonContext = null;
            if (context is IronPythonScriptExecutionContext)
            {
                ironPythonContext = (IronPythonScriptExecutionContext)context;
            }
            else throw new ScriptException("context must be an IronPython context.");
            AddPath(Path.GetDirectoryName(Path.GetFullPath(path)));
            var scope = ScriptingEngine.ExecuteFile(path, ironPythonContext.CurrentScriptScope);
            RemovePath(Path.GetDirectoryName(Path.GetFullPath(path)));
            return new IronPythonScriptExecutionContext(scope);
        }
        public T ExecuteString<T>(string content, IScriptExecutionContext context)
        {
            IronPythonScriptExecutionContext ironPythonContext = null;
            if (context is IronPythonScriptExecutionContext)
            {
                ironPythonContext = (IronPythonScriptExecutionContext)context;
            }
            else throw new ScriptException("context must be an IronPython context.");
            return ScriptingEngine.Execute<T>(content, ironPythonContext.CurrentScriptScope);
        }
        public T ExecuteString<T>(string content)
        {
            return ExecuteString<T>(content, new IronPythonScriptExecutionContext(ScriptingEngine));
        }

        public void SetIn(TextReader reader)
        {
            ScriptingEngine.Runtime.IO.SetInput(new MemoryStream(), reader, Encoding.Default);
        }

        public void SetOut(TextWriter writer)
        {
            ScriptingEngine.Runtime.IO.SetOutput(new MemoryStream(), writer);
        }

        public void SetError(TextWriter writer)
        {
            ScriptingEngine.Runtime.IO.SetErrorOutput(new MemoryStream(), writer);
        }

        public IScriptExecutionContext CreateModule(string name)
        {
            return new IronPythonScriptExecutionContext(ScriptingEngine.CreateModule(name));
        }

        public IScriptExecutionContext GetDefaultModule()
        {
            return new IronPythonScriptExecutionContext(ScriptingEngine.GetBuiltinModule());
        }

        public IScriptExecutionContext CreateContext()
        {
            return new IronPythonScriptExecutionContext(ScriptingEngine);
        }

        public void RemovePath(string path)
        {
            List<string> paths = ScriptingEngine.GetSearchPaths().ToList();
            paths.Where(s => s == path).ToList().ForEach(s => paths.Remove(s));
            ScriptingEngine.SetSearchPaths(paths);
        }

        public IScriptExecutionContext GetContext(string name)
        {
            return _storedContexts[name];
        }

        public IScriptExecutionContext AddContext(string name)
        {
            if(_storedContexts.ContainsKey(name))
            {
                RemoveContext(name);
            }
            IronPythonScriptExecutionContext context = new IronPythonScriptExecutionContext(ScriptingEngine);
            _storedContexts.Add(name, context);
            return context;
        }

        public void RemoveContext(string name)
        {
            _storedContexts.Remove(name);
        }

        public void AddAssembly(Assembly assembly)
        {
            ScriptingEngine.Runtime.LoadAssembly(assembly);
        }
    }
}