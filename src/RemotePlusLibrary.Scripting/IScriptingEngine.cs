using Microsoft.Scripting.Hosting;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace RemotePlusLibrary.Scripting
{
    public interface IScriptingEngine
    {
        #region Engine Methods
        void SetIn(TextReader reader);
        void SetOut(TextWriter writer);
        void SetError(TextWriter writer);
        IScriptExecutionContext ExecuteFile(string path);
        IScriptExecutionContext ExecuteFile(string path, IScriptExecutionContext context);
        IScriptExecutionContext CreateModule(string name);
        IScriptExecutionContext GetDefaultModule();
        IScriptExecutionContext CreateContext();
        T ExecuteString<T>(string content);
        T ExecuteString<T>(string content, IScriptExecutionContext context);
        void AddAssembly(Assembly assembly);
        void AddPath(string path);
        void RemovePath(string path);
        #endregion
        #region Context Methods
        IScriptExecutionContext GetContext(string name);
        IScriptExecutionContext AddContext(string name);
        void RemoveContext(string name);
        #endregion
    }
}