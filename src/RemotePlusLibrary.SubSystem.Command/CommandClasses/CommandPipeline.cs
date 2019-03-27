using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.SubSystem.Command.CommandClasses
{
    [CollectionDataContract]
    public class CommandPipeline : IList<CommandRoutine>
    {
        private List<CommandRoutine> _internalList;
        public CommandPipeline()
        {
            _internalList = new List<CommandRoutine>();
        }

        #region IDictionary
        public CommandRoutine this[int commandPosition] { get => _internalList[commandPosition]; set => _internalList[commandPosition] = value; }
        [DataMember]

        public int Count => _internalList.Count;
        [DataMember]

        public bool IsReadOnly => true;

        public void Add(CommandRoutine routine)
        {
            _internalList.Add(routine);
        }

        public void Clear()
        {
            _internalList.Clear();
        }

        public bool Contains(CommandRoutine command)
        {
            return _internalList.Contains(command);
        }

        public void CopyTo(CommandRoutine[] array, int arrayIndex)
        {
            _internalList.Add(array[arrayIndex]);
        }

        public IEnumerator<CommandRoutine> GetEnumerator()
        {
            return _internalList.GetEnumerator();
        }

        public void RemoveAt(int commandPosition)
        {
            _internalList.RemoveAt(commandPosition);
        }

        public bool Remove(CommandRoutine item)
        {
            return _internalList.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _internalList.GetEnumerator();
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

        public int IndexOf(CommandRoutine item)
        {
            return _internalList.IndexOf(item);
        }

        public void Insert(int index, CommandRoutine routine)
        {
            _internalList.Insert(index, routine);
        }
    }
}