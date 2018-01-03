using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.CommandSystem.CommandClasses
{
    [CollectionDataContract]
    public class CommandPipeline : IDictionary<int, CommandRoutine>
    {
        private Dictionary<int, CommandRoutine> _internalDictionary;
        private Dictionary<int, CommandRoutine> _subRoutines;
        public CommandPipeline()
        {
            _internalDictionary = new Dictionary<int, CommandRoutine>();
            _subRoutines = new Dictionary<int, CommandRoutine>();
        }

        #region IDictionary
        public CommandRoutine this[int commandPosition] { get => _internalDictionary[commandPosition]; set => _internalDictionary[commandPosition] = value; }
        [DataMember]
        public ICollection<int> Keys => _internalDictionary.Keys;
        [DataMember]

        public ICollection<CommandRoutine> Values => _internalDictionary.Values;
        [DataMember]

        public int Count => _internalDictionary.Count;
        [DataMember]

        public bool IsReadOnly => true;

        public void Add(int commandPosition, CommandRoutine routine)
        {
            _internalDictionary.Add(commandPosition, routine);
        }
        public void AddSubRoutine(int commandPosition, CommandRoutine sub)
        {
            _subRoutines.Add(commandPosition, sub);
        }
        public void Add(KeyValuePair<int, CommandRoutine> commands)
        {
            _internalDictionary.Add(commands.Key, commands.Value);
        }

        public void Clear()
        {
            _internalDictionary.Clear();
        }

        public bool Contains(KeyValuePair<int, CommandRoutine> command)
        {
            return _internalDictionary.Contains(command);
        }

        public bool ContainsKey(int commandPosition)
        {
            return _internalDictionary.ContainsKey(commandPosition);
        }

        public void CopyTo(KeyValuePair<int, CommandRoutine>[] array, int arrayIndex)
        {
            _internalDictionary.Add(array[arrayIndex].Key, array[arrayIndex].Value);
        }

        public IEnumerator<KeyValuePair<int, CommandRoutine>> GetEnumerator()
        {
            return _internalDictionary.GetEnumerator();
        }

        public bool Remove(int commandPosition)
        {
            return _internalDictionary.Remove(commandPosition);
        }

        public bool Remove(KeyValuePair<int, CommandRoutine> item)
        {
            return _internalDictionary.Remove(item.Key);
        }

        public bool TryGetValue(int commandPosition, out CommandRoutine value)
        {
            return _internalDictionary.TryGetValue(commandPosition, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _internalDictionary.GetEnumerator();
        }
#endregion IDictionary

        /// <summary>
        /// Gets the latest command in the command pipeline.
        /// </summary>
        /// <returns>The command routine that represents the latest command.</returns>
        public CommandRoutine GetLatest()
        {
            return this[Count - 1];
        }
    }
}