using IronPython.Hosting;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using RemotePlusLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Scripting
{
    public class ScriptBuilder : IScriptingEngine
    {
        private  Dictionary<string, ScriptGlobal> globals = new Dictionary<string, ScriptGlobal>();
        public const string SCRIPT_LOG_CONSTANT = "Script Engine";
        public ScriptEngine ScriptingEngine { get; private set; }
        public ScriptScope _templateScope = null;
        public void InitializeGlobals()
        {
            foreach (KeyValuePair<string, ScriptGlobal> global in globals)
            {
                ScriptingEngine.GetBuiltinModule().SetVariable(global.Key, global.Value.Global);
            }
        }
        public void AddAssembly(string name)
        {
            ScriptingEngine.Execute($"clr.AddReference(\"{name}\")", _templateScope);
        }
        public void AddClass<TClass>()
        {
            ScriptingEngine.Execute($"from {typeof(TClass).Namespace} import {typeof(TClass).Name}", _templateScope);
        }
        public void AddScriptObject<T>(string objectName, T scriptObject, string description, ScriptGlobalType objectType) where T : class
        {
            var newGlobal = new ScriptGlobal(scriptObject) { Information = new ScriptGlobalInformation() { Name = objectName, Description = description, Type = objectType } };
            FillMembersRecurs(newGlobal);
            globals.Add(objectName, newGlobal);
            if(ScriptingEngine != null)
            {
                ScriptingEngine.GetBuiltinModule().SetVariable(objectName, newGlobal.Global);
            }
        }
        public void ImportModule(string module)
        {
            ScriptingEngine.ImportModule(module);
        }
        private void FillMembersRecurs(ScriptGlobal newGlobal)
        {
            FillData(newGlobal.Global.GetType(), newGlobal.Information);
        }
        void FillData(Type t, ScriptGlobalInformation i)
        {
            foreach (MethodInfo method in t.GetMethods().Where(type => type.GetCustomAttribute<IndexScriptObjectAttribute>() != null))
            {
                ScriptGlobalInformation info = new ScriptGlobalInformation();
                var g = new ScriptGlobalInformation();
                g.Name = method.Name;
                g.Type = ScriptGlobalType.Function;
                i.Members.Add(g);
            }
            foreach (var property in t.GetProperties().Where(type => type.GetCustomAttribute<IndexScriptObjectAttribute>() != null))
            {
                ScriptGlobalInformation info = new ScriptGlobalInformation();
                info.Name = property.Name;
                info.Type = ScriptGlobalType.Variable;
                FillDataProperty(property.PropertyType, info);
                i.Members.Add(info);
            }
        }
        void FillDataProperty(Type t, ScriptGlobalInformation i)
        {
            ScriptGlobalInformation info = new ScriptGlobalInformation();
            foreach (var property in t.GetProperties().Where(type => type.GetCustomAttribute<IndexScriptObjectAttribute>() != null))
            {
                info.Name = property.Name;
                info.Type = ScriptGlobalType.Variable;
                i.Members.Add(info);
                FillDataProperty(property.PropertyType, info);
            }
        }
        public bool FunctionExists(string functionName)
        {
            return globals.ContainsKey(functionName);
        }
        public List<ScriptGlobal> GetGlobals()
        {
            return globals.Values.ToList();
        }

        public void InitializeEngine()
        {
            ScriptingEngine = Python.CreateEngine();
            var paths = ScriptingEngine.GetSearchPaths();
            paths.Add($"{Environment.CurrentDirectory}\\extensions");
            ScriptingEngine.SetSearchPaths(paths);
            ScriptingEngine.GetBuiltinModule().ImportModule("clr");
            _templateScope = ScriptingEngine.CreateScope();
            InitializeGlobals();
        }
        public object ExecuteString(string script)
        {
            try
            {
                var source = ScriptingEngine.CreateScriptSourceFromString(script);
                return source.Execute(_templateScope);
            }
            catch (Exception ex)
            {
                throw new ScriptException($"There was an error while executing a script. Error: {ex.Message}", ex);
            }
        }
        ScriptScope staticScope = null;
        public object ExecuteStringUsingSameScriptScope(string script)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(script))
                {
                    throw new Exception("Script is empty.");
                }
                var source = ScriptingEngine.CreateScriptSourceFromString(script);
                if (staticScope == null)
                {
                    staticScope = _templateScope;
                }
                return source.Execute(staticScope);
            }
            catch (Exception ex)
            {
                throw new ScriptException($"There was an error while executing a script. Error: {ex.Message}", ex);
            }
        }
        public void ClearStaticScope()
        {
            staticScope = _templateScope;
        }
    }
}