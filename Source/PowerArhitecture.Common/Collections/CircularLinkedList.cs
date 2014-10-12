using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PowerArhitecture.Common.Collections
{
    public class CircularLinkedList<T> : IEnumerable<T> {
        private readonly LinkedList<T> _list = new LinkedList<T>();
        private readonly int _capacity;

        public CircularLinkedList(int capacity)
        {
            _capacity = capacity;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Add(T e) {
            _list.AddFirst(e);
            if (_list.Count > _capacity)
                _list.RemoveLast();
        }

        public IEnumerator<T> GetEnumerator() {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
