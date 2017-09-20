using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace RemotePlusLibrary.Converters
{
    [DataContract]
    [Serializable]
    public class UserAccountCollectionDescriptor : PropertyDescriptor
    {
        [DataMember]
        private UserCollection collection = null;
        [DataMember]
        private int index = -1;
        public UserAccountCollectionDescriptor(UserCollection coll, int idx) : base("#" + idx.ToString(), null)
        {
            this.collection = coll;
            this.index = idx;
        }
        [DataMember]
        public override AttributeCollection Attributes
        {
            get
            {
                return new AttributeCollection(null);
            }
        }
        public override bool CanResetValue(object component)
        {
            return true;
        }
        [DataMember]
        public override Type ComponentType
        {
            get
            {
                return this.collection.GetType();
            }
        }
        [DataMember]
        public override string DisplayName
        {
            get
            {
                UserAccount uc = this.collection.Get(index);
                return uc.Credentials.Username;
            }
        }
        public override object GetValue(object component)
        {
            return this.collection.Get(index);
        }
        [DataMember]
        public override bool IsReadOnly
        {
            get
            {
                return false;
            }
        }
        [DataMember]
        public override string Name
        {
            get
            {
                return "#" + index.ToString();
            }
        }
        [DataMember]
        public override Type PropertyType
        {
            get
            {
                return this.collection.Get(index).GetType();
            }
        }
        public override void ResetValue(object component)
        {
    
        }
        public override void SetValue(object component, object value)
        {
            collection.Set(index, (UserAccount)value);
        }

        public override bool ShouldSerializeValue(object component)
        {
            return true;
        }
    }
}