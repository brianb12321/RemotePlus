using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Core
{
    public static class DefaultKnownTypeManager
    {
        private static List<Type> _knownType = new List<Type>();
        private static List<string> _knownTypeNames = new List<string>();
        public static IEnumerable<Type> GetKnownTypes(ICustomAttributeProvider provider)
        {
            return _knownType;
        }
        public static void AddType(Type t)
        {
            _knownType.Add(t);
            _knownTypeNames.Add(t.Name);
        }
        public static IEnumerable<string> GetKnownTypeNames()
        {
            return _knownTypeNames;
        }
        public static bool HasName(string name)
        {
            if(_knownTypeNames.Contains(name))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static void LoadDefaultTypes()
        {
            AddType(typeof(int));
            AddType(typeof(string));
            AddType(typeof(List<int>));
            AddType(typeof(List<string>));
            AddType(typeof(Dictionary<string, string>));
            AddType(typeof(Color));
        }
    }
}
