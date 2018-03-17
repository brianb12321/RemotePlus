using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using RemotePlusLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Scripting
{
    public class ScriptBuilder
    {
        private  Dictionary<string, ScriptGlobal> globals = new Dictionary<string, ScriptGlobal>();
        public const string SCRIPT_LOG_CONSTANT = "Script Engine";
        public ScriptEngine ScriptingEngine { get; private set; }
        ScriptScope InitializeScript()
        {
            var scope = ScriptingEngine.CreateScope();
            foreach (KeyValuePair<string, ScriptGlobal> global in globals)
            {
                scope.SetVariable(global.Key, global.Value.Global);
            }
            return scope;
        }
        public void AddScriptObject<T>(string objectName, T scriptObject, string description, ScriptGlobalType objectType) where T : class
        {
            var newGlobal = new ScriptGlobal(scriptObject) { Information = new ScriptGlobalInformation() { Name = objectName, Description = description, Type = objectType } };
            FillMembersRecurs(newGlobal);
            globals.Add(objectName, newGlobal);
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
        }
        public bool ExecuteString(string script)
        {
            var source = ScriptingEngine.CreateScriptSourceFromString(script);
            return source.Execute(InitializeScript());
        }
    }
}