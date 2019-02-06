using System;
using System.Runtime.Serialization;

namespace RemotePlusLibrary.Extension.ResourceSystem.ResourceTypes
{
    public delegate bool DevicePropertyChangedEventHandler(object sender, object value);
    public delegate object DevicePropertyReadEventHandler();
    [Serializable]
    [DataContract]
    public class DeviceProperty
    {
        [DataMember]
        public string Name { get; private set; }
        [DataMember]
        private object _value = null;
        [DataMember]
        public object Value
        {
            get
            {
                if(StaticData)
                {
                    return _value;
                }
                return PropertyRead.Invoke();
            }
            private set
            {
                _value = value;
            }
        }
        [DataMember]
        public bool CanEdit { get; private set; }
        [DataMember]
        public bool StaticData { get; private set; }
        public event DevicePropertyChangedEventHandler PropertyChanged;
        public event DevicePropertyReadEventHandler PropertyRead;
        public DeviceProperty(bool staticData, bool editable, string name)
        {
            CanEdit = editable;
            Name = name;
            StaticData = staticData;
        }
        public DeviceProperty(bool staticData, bool editable, string name, object value)
        {
            Name = name;
            Value = value;
            CanEdit = editable;
            StaticData = staticData;
        }
        public bool SetValue(object value)
        {
            if (!CanEdit) return false;
            bool? shouldChange = PropertyChanged?.Invoke(this, value);
            if (shouldChange.HasValue)
            {
                if (shouldChange.Value)
                {
                    Value = value;
                    return true;
                }
                else { return false; }
            }
            else
            {
                Value = value;
                return true;
            }
        }
        public override string ToString()
        {
            return Value.ToString();
        }
    }
}