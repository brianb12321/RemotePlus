using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Editors.PolicyManagerClasses
{
    public static class PolicyEditors
    {
        private static Dictionary<string, IPolicyEditor> editors = new Dictionary<string, IPolicyEditor>();
        public static IPolicyEditor Get(string name)
        {
            return editors[name];
        }
        public static void Add(string name, IPolicyEditor editor)
        {
            editors.Add(name, editor);
        }
    }
}
