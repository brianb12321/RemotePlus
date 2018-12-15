using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Scripting
{
    /// <summary>
    /// Stores all the variable and function declarations.
    /// </summary>
    public class ScriptContext
    {
        ScriptScope _scope;
        ScriptEngine _engine;
        public void Clear()
        {
            _scope = _engine.CreateScope();
        }
        public ScriptContext(ScriptEngine e)
        {
            _engine = e;
        }
        public ScriptScope GetScope()
        {
            return _scope;
        }
    }
}