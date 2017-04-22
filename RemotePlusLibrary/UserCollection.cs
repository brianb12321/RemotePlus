using RemotePlusLibrary.Converters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary
{
    [Serializable]
    [CollectionDataContract]
    [KnownType(typeof(UserAccount))]
    public class UserCollection : CollectionBase, ICustomTypeDescriptor
    {
        public UserAccount this[int index]
        {
            get
            {
                return (UserAccount)List[index];
            }
            set
            {
                List[index] = value;
            }
        }
        public UserAccount Get(int index)
        {
            return (UserAccount)List[index];
        }
        public void Set(int index, UserAccount a)
        {
            List[index] = a;
        }
        public void Add(UserAccount uc)
        {
            List.Add(uc);
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
            for(int i = 0; i < this.List.Count; i++)
            {
                UserAccountCollectionDescriptor pd = new UserAccountCollectionDescriptor(this, i);
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

        public void Remove(UserAccount uc)
        {
            List.Remove(uc);
        }
    }
}
