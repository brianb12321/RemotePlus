using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using IronPython;

namespace RemotePlusLibrary.Scripting
{
    public class IronPythonScriptExecutionContext : IScriptExecutionContext
    {
        public ScriptScope CurrentScriptScope { get; set; } = null;

        public IronPythonScriptExecutionContext(ScriptEngine engine)
        {
            CurrentScriptScope = engine.CreateScope();
        }
        public IronPythonScriptExecutionContext(ScriptScope scope)
        {
            CurrentScriptScope = scope;
        }
        public void AddVariable<T>(string name, T variable)
        {
            CurrentScriptScope.SetVariable(name, variable);
        }

        public bool ContainsVariable(string name)
        {
            return CurrentScriptScope.ContainsVariable(name);
        }

        public string[] GetAllVariableNames()
        {
            return CurrentScriptScope.GetVariableNames().ToArray();
        }

        public T[] GetAllVariablesByType<T>()
        {
            return CurrentScriptScope.GetVariableNames()
                .Where(s => CurrentScriptScope.GetVariable(s) is T)
                .Select(s => GetVariable<T>(s))
                .ToArray();
        }

        public T GetVariable<T>(string name)
        {
            return CurrentScriptScope.GetVariable<T>(name);
        }

        void IExtension<InstanceContext>.Attach(InstanceContext owner)
        {
            
        }

        void IExtension<InstanceContext>.Detach(InstanceContext owner)
        {
            
        }
    }
}