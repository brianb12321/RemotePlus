using RemotePlusLibrary.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Scripting
{
    /// <summary>
    /// Exposes methods for executing scripts and setting script contexts.
    /// </summary>
    public interface IScriptManager
    {
        TReturn RunString<TReturn>(string scriptContent, string context);
        void AddScriptItemToGlobal<TItem>(string name, TItem item);
        void AddScriptType<TType>(TType ns);
        void ImportModule(string module);
        void AddScriptContext(string contextName);
        void SetStandordOutStream(ITextStream stream);
        void SetStandordInStream(ITextStream stream);
        void SetStandordErrorStream(ITextStream stream);
    }
}