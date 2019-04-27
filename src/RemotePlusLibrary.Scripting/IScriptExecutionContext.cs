using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Scripting
{
    public interface IScriptExecutionContext : IExtension<InstanceContext>
    {
        bool ContainsVariable(string name);
        T GetVariable<T>(string name);
        void AddVariable<T>(string name, T variable);
        string[] GetAllVariableNames();
        T[] GetAllVariablesByType<T>();
    }
}