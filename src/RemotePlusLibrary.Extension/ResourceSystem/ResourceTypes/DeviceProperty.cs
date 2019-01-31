using System;
using System.Runtime.Serialization;

namespace RemotePlusLibrary.Extension.ResourceSystem.ResourceTypes
{
    public delegate bool DevicePropertyChangedEventHandler(object sender, object value);
    [Serializable]
    [DataContract]
    public class DeviceProperty
    {
        [DataMember]
        public string Name { get; private set; }
        [DataMember]
        public object Value { get; private set; }
        [DataMember]
        public bool CanEdit { get; private set; }
        public event DevicePropertyChangedEventHandler PropertyChanged;
        public DeviceProperty(bool editable, string name)
        {
            CanEdit = editable;
            Name = name;
        }
        public DeviceProperty(bool editable, string name, object value)
        {
            Name = name;
            Value = value;
            CanEdit = editable;
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