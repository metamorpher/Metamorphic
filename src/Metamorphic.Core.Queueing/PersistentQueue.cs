using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Metamorphic.Core.Queueing
{
    internal sealed class PersistentQueue<T> : IPersistentQueue<T> where T : ISerializable
    {


        internal PersistentQueue()
        {
        }

        /// <summary>
        /// Removes an item from the queue for processing.
        /// </summary>
        /// <returns>The item.</returns>
        public T Dequeue()
        {
        }

        /// <summary>
        /// Adds the given item to the queue for processing.
        /// </summary>
        /// <param name="signal">The item.</param>
        public void Enqueue(T item)
        {
        }

        /// <summary>
        /// Gets a value indicating whether the queue is currently empty.
        /// </summary>
        public bool IsEmpty
        {
            get;
        }

        /// <summary>
        /// An event raised when a new item is enqueued.
        /// </summary>
        public event EventHandler OnEnqueue;
    }
}
