using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.CommandSystem.CommandClasses.Parsing
{
    public class ElementSet : IList<ICommandElement>
    {
        List<ICommandElement> _internalList;

        public ElementSet()
        {
            _internalList = new List<ICommandElement>();
        }

        public ICommandElement this[int index] { get => _internalList[index]; set => _internalList[index] = value; }

        public int Count => _internalList.Count;

        public bool IsReadOnly => false;

        public void Add(ICommandElement token)
        {
            _internalList.Add(token);
        }

        public void Clear()
        {
            _internalList.Clear();
        }

        public bool Contains(ICommandElement token)
        {
            return _internalList.Contains(token);
        }

        public void CopyTo(ICommandElement[] array, int arrayIndex)
        {
            _internalList.CopyTo(array, arrayIndex);
        }

        public IEnumerator<ICommandElement> GetEnumerator()
        {
            return _internalList.GetEnumerator();
        }

        public int IndexOf(ICommandElement token)
        {
            return _internalList.IndexOf(token);
        }

        public void Insert(int index, ICommandElement token)
        {
            _internalList.Insert(index, token);
        }

        public bool Remove(ICommandElement token)
        {
            return _internalList.Remove(token);
        }

        public void RemoveAt(int index)
        {
            _internalList.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _internalList.GetEnumerator();
        }
        public ICommandElement GetFlag(string flag)
        {
            return _internalList.First((t) => t.Value.ToString() == flag);
        }
        public bool HasFlag(string flag)
        {
            try
            {
                _internalList.First((t) => t.Value.ToString() == flag);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}