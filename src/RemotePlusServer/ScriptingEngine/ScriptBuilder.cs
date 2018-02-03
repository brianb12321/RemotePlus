using MoonSharp.Interpreter;
using RemotePlusLibrary;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using RemotePlusServer.ScriptingEngine.Proxies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusServer.ScriptingEngine
{
    public static class ScriptBuilder
    {
        private static Dictionary<string, object> globals = new Dictionary<string, object>();
        public const string SCRIPT_LOG_CONSTANT = "Script Engine";
        internal static void InitializeGlobals()
        {
            try
            {
                globals.Add("serverInstance", new LuaServerInstance());
                globals.Add("executeServerCommand", new Func<string, CommandPipeline>((command => ServerManager.DefaultService.Remote.RunServerCommand(command, RemotePlusLibrary.Extension.CommandSystem.CommandExecutionMode.Script))));
                globals.Add("speak", new Action<string, int, int>(StaticRemoteFunctions.speak));
                globals.Add("beep", new Action<int, int>(StaticRemoteFunctions.beep));
                globals.Add("functionExists", new Func<string, bool>((name) => FunctionExists(name)));
                globals.Add("createRequestBuilder", new Func<string, string, Dictionary<string, string>, RequestBuilder>(ClientInstance.createRequestBuilder));
            }
            catch (ArgumentException)
            {

            }
        }
        public static void InitializeScripts(Script luaScript)
        {
            InitializeGlobals();
            foreach(KeyValuePair<string, object> global in globals)
            {
                luaScript.Globals[global.Key] = global.Value;
            }
        }
        internal static void RegisterUserData()
        {
            UserData.RegisterType<LuaServerInstance>();
            UserData.RegisterType<ClientInstance>();
            UserData.RegisterType<CommandResponse>();
            UserData.RegisterType<CommandPipeline>();
            UserData.RegisterType<CommandRequest>();
            UserData.RegisterType<CommandRoutine>();
            UserData.RegisterType<RequestBuilder>();
            UserData.RegisterType<ReturnData>();
        }
        public static void ConfigureScript(Script luaScript)
        {
            luaScript.Options.DebugPrint = (i) => ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(i + Environment.NewLine);
        }
        public static void AddUserData<T>()
        {
            UserData.RegisterType<T>();
        }
        public static void AddFunction<T>(string functionName, T function) where T : class
        {
            if(!typeof(T).IsSubclassOf(typeof(Delegate)))
            {
                throw new Exception("You have to provide a method.");
            }
            else
            {
                globals.Add(functionName, function);
            }
        }
        public static bool FunctionExists(string functionName)
        {
            return globals.ContainsKey(functionName);
        }
    }
}