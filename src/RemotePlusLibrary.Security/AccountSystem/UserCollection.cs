using RemotePlusLibrary.Security.AccountSystem.Design.Converters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Security.AccountSystem
{
    [Serializable]
    [CollectionDataContract]
    public class UserCollection : IDictionary<Guid, UserAccount>, ICustomTypeDescriptor
    {
        Dictionary<Guid, UserAccount> _account = new Dictionary<Guid, UserAccount>();
        public UserAccount this[Guid AID]
        {
            get
            {
                return _account[AID];
            }
            set
            {
                _account[AID] = value;
            }
        }

        public ICollection<Guid> Keys => ((IDictionary<Guid, UserAccount>)_account).Keys;

        public ICollection<UserAccount> Values => ((IDictionary<Guid, UserAccount>)_account).Values;

        public int Count => ((IDictionary<Guid, UserAccount>)_account).Count;

        public bool IsReadOnly => ((IDictionary<Guid, UserAccount>)_account).IsReadOnly;

        public void Add(Guid key, UserAccount value)
        {
            ((IDictionary<Guid, UserAccount>)_account).Add(key, value);
        }

        public void Add(KeyValuePair<Guid, UserAccount> item)
        {
            ((IDictionary<Guid, UserAccount>)_account).Add(item);
        }

        public void Clear()
        {
            ((IDictionary<Guid, UserAccount>)_account).Clear();
        }

        public bool Contains(KeyValuePair<Guid, UserAccount> item)
        {
            return ((IDictionary<Guid, UserAccount>)_account).Contains(item);
        }

        public bool ContainsKey(Guid key)
        {
            return ((IDictionary<Guid, UserAccount>)_account).ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<Guid, UserAccount>[] array, int arrayIndex)
        {
            ((IDictionary<Guid, UserAccount>)_account).CopyTo(array, arrayIndex);
        }

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public IEnumerator<KeyValuePair<Guid, UserAccount>> GetEnumerator()
        {
            return ((IDictionary<Guid, UserAccount>)_account).GetEnumerator();
        }

        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        public PropertyDescriptorCollection GetProperties()
        {
            var pds = new PropertyDescriptorCollection(null);
            foreach(Guid g in _account.Keys)
            {
                UserAccountCollectionDescriptor pd = new UserAccountCollectionDescriptor(this, g);
                pds.Add(pd);
            }
            return pds;
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return GetProperties();
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        public bool Remove(Guid key)
        {
            return ((IDictionary<Guid, UserAccount>)_account).Remove(key);
        }

        public bool Remove(KeyValuePair<Guid, UserAccount> item)
        {
            return ((IDictionary<Guid, UserAccount>)_account).Remove(item);
        }

        public bool TryGetValue(Guid key, out UserAccount value)
        {
            return ((IDictionary<Guid, UserAccount>)_account).TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IDictionary<Guid, UserAccount>)_account).GetEnumerator();
        }
    }
}
