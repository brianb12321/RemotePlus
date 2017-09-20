using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace RemotePlusLibrary
{
    [DataContract]
    public class VariableManager : IDictionary<string, string>
    {
        [DataMember]
        Dictionary<string, string> vars;
        private VariableManager()
        {
            vars = new Dictionary<string, string>();
        }

        public string this[string key]
        {
            get
            {
                return vars[key];
            }
            set
            {
                vars[key] = value;
            }
        }
        [IgnoreDataMember]
        public ICollection<string> Keys => vars.Keys;
        [IgnoreDataMember]
        public ICollection<string> Values => vars.Values;
        [IgnoreDataMember]
        public int Count => vars.Count;
        [IgnoreDataMember]
        public bool IsReadOnly => false;

        public static VariableManager Load()
        {
            DataContractSerializer ser = new DataContractSerializer(typeof(VariableManager), Core.DefaultKnownTypeManager.GetKnownTypes(null));
            XmlReader reader = XmlReader.Create("Variables.xml");
            var ss = (VariableManager)ser.ReadObject(reader);
            reader.Close();
            return ss;
        }
        public static VariableManager New()
        {
            VariableManager vm = new VariableManager();
            return vm;
        }

        public void Add(string key, string value)
        {
            vars.Add(key, value);
        }

        public void Add(KeyValuePair<string, string> item)
        {
            vars.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            vars.Clear();
        }

        public bool Contains(KeyValuePair<string, string> item)
        {
            return vars.Contains(item);
        }

        public bool ContainsKey(string key)
        {
            return vars.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return vars.GetEnumerator();
        }

        public bool Remove(string key)
        {
            return vars.Remove(key);
        }

        public bool Remove(KeyValuePair<string, string> item)
        {
            return vars.Remove(item.Key);
        }

        public bool TryGetValue(string key, out string value)
        {
            return vars.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return vars.GetEnumerator();
        }
        public void Save()
        {
            DataContractSerializer xsSubmit = new DataContractSerializer(typeof(VariableManager), Core.DefaultKnownTypeManager.GetKnownTypes(null));
            var subReq = this;
            XmlWriterSettings settings = new XmlWriterSettings()
            {
                Indent = true
            };
            using (var sww = new StringWriter())
            using (XmlWriter writer = XmlWriter.Create("Variables.xml", settings))
            {
                xsSubmit.WriteObject(writer, subReq);
            }
        }
    }
}
