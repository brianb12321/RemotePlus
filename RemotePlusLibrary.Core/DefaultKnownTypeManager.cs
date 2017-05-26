using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Core
{
    public static class DefaultKnownTypeManager
    {
        private static List<Type> _knownType = new List<Type>();
        public static IEnumerable<Type> GetKnownTypes(ICustomAttributeProvider provider)
        {
            return _knownType;
        }
        public static void AddType(Type t)
        {
            _knownType.Add(t);
        }
        public static void LoadDefaultTypes()
        {
            _knownType.Add(typeof(int));
            _knownType.Add(typeof(string));
            _knownType.Add(typeof(List<int>));
            _knownType.Add(typeof(List<string>));
            _knownType.Add(typeof(Dictionary<string, string>));
        }
    }
}
