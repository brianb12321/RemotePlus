using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.CommandSystem.CommandClasses.Parsing
{
    public class TokenSet : IList<CommandToken>
    {
        List<CommandToken> _internalList;

        public TokenSet()
        {
            _internalList = new List<CommandToken>();
        }

        public CommandToken this[int index] { get => _internalList[index]; set => _internalList[index] = value; }

        public int Count => _internalList.Count;

        public bool IsReadOnly => false;

        public void Add(CommandToken token)
        {
            _internalList.Add(token);
        }

        public void Clear()
        {
            _internalList.Clear();
        }

        public bool Contains(CommandToken token)
        {
            return _internalList.Contains(token);
        }

        public void CopyTo(CommandToken[] array, int arrayIndex)
        {
            _internalList.CopyTo(array, arrayIndex);
        }

        public IEnumerator<CommandToken> GetEnumerator()
        {
            return _internalList.GetEnumerator();
        }

        public int IndexOf(CommandToken token)
        {
            return _internalList.IndexOf(token);
        }

        public void Insert(int index, CommandToken token)
        {
            _internalList.Insert(index, token);
        }

        public bool Remove(CommandToken token)
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
        public CommandToken GetFlag(string flag)
        {
            return _internalList.First((t) => t.Value == flag);
        }
        public bool HasFlag(string flag)
        {
            try
            {
                _internalList.First((t) => t.Value == flag);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}