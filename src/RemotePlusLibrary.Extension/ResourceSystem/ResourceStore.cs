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

namespace RemotePlusLibrary.Extension.ResourceSystem
{
    [DataContract]
    public class ResourceStore : IDictionary<string, Resource>
    {
        [DataMember]
        Dictionary<string, Resource> resources;
        private ResourceStore()
        {
            resources = new Dictionary<string, Resource>();
        }

        public Resource this[string key]
        {
            get
            {
                return resources[key];
            }
            set
            {
                resources[key] = value;
            }
        }
        [IgnoreDataMember]
        public ICollection<string> Keys => resources.Keys;
        [IgnoreDataMember]
        public ICollection<Resource> Values => resources.Values;
        [IgnoreDataMember]
        public int Count => resources.Count;
        [IgnoreDataMember]
        public bool IsReadOnly => false;

        public static ResourceStore Load()
        {
            DataContractSerializer ser = new DataContractSerializer(typeof(ResourceStore), Core.DefaultKnownTypeManager.GetKnownTypes(null));
            XmlReader reader = XmlReader.Create("Variables.xml");
            var ss = (ResourceStore)ser.ReadObject(reader);
            reader.Close();
            return ss;
        }
        public static ResourceStore New()
        {
            ResourceStore vm = new ResourceStore();
            return vm;
        }

        public void Add(string key, Resource value)
        {
            resources.Add(key, value);
        }

        public void Add(KeyValuePair<string, Resource> item)
        {
            resources.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            resources.Clear();
        }

        public bool Contains(KeyValuePair<string, Resource> item)
        {
            return resources.Contains(item);
        }

        public bool ContainsKey(string key)
        {
            return resources.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, Resource>[] array, int arrayIndex)
        {
            
        }

        public IEnumerator<KeyValuePair<string, Resource>> GetEnumerator()
        {
            return resources.GetEnumerator();
        }

        public bool Remove(string key)
        {
            return resources.Remove(key);
        }

        public bool Remove(KeyValuePair<string, Resource> item)
        {
            return resources.Remove(item.Key);
        }

        public bool TryGetValue(string key, out Resource value)
        {
            return resources.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return resources.GetEnumerator();
        }
        public void Save()
        {
            DataContractSerializer xsSubmit = new DataContractSerializer(typeof(ResourceStore), Core.DefaultKnownTypeManager.GetKnownTypes(null));
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
