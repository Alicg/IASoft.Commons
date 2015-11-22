﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Utils.DataStrucures.LinkedLists
{
    /// <summary>
    /// Represents a circular doubly linked list.
    /// </summary>
    /// <typeparam name="T">Specifies the element type of the linked list.</typeparam>
    [DebuggerDisplay("Count = {Count}")]
    public sealed class CircularLinkedList<T> : ICollection<T>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        Node<T> _head;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        Node<T> _tail;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        int _count;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly IEqualityComparer<T> _comparer;

        /// <summary>
        /// Initializes a new instance of CircularLinkedList
        /// </summary>
        public CircularLinkedList()
            : this(null, EqualityComparer<T>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of CircularLinkedList
        /// </summary>
        /// <param name="collection">Collection of objects that will be added to linked list</param>
        public CircularLinkedList(IEnumerable<T> collection)
            : this(collection, EqualityComparer<T>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of CircularLinkedList
        /// </summary>
        /// <param name="comparer">Comparer used for item comparison</param>
        /// <exception cref="ArgumentNullException">comparer is null</exception>
        public CircularLinkedList(IEqualityComparer<T> comparer)
            : this(null, comparer)
        {
        }

        /// <summary>
        /// Initializes a new instance of CircularLinkedList
        /// </summary>
        /// <param name="collection">Collection of objects that will be added to linked list</param>
        /// <param name="comparer">Comparer used for item comparison</param>
        public CircularLinkedList(IEnumerable<T> collection, IEqualityComparer<T> comparer)
        {
            _tail = null;
            if (comparer == null)
                throw new ArgumentNullException("comparer");
            _comparer = comparer;
            if (collection == null) return;
            foreach (var item in collection)
                AddLast(item);
        }

        /// <summary>
        /// Gets Tail node. Returns NULL if no tail node found
        /// </summary>
        public Node<T> Tail { get { return _tail; } }

        /// <summary>
        /// Gets the head node. Returns NULL if no node found
        /// </summary>
        public Node<T> Head { get { return _head; } }

        /// <summary>
        /// Gets total number of items in the list
        /// </summary>
        public int Count { get { return _count; } }

        /// <summary>
        /// Gets the item at the current index
        /// </summary>
        /// <param name="index">Zero-based index</param>
        /// <exception cref="ArgumentOutOfRangeException">index is out of range</exception>
        public Node<T> this[int index]
        {
            get
            {
                if (index >= _count || index < 0)
                    throw new ArgumentOutOfRangeException("index");
                var node = _head;
                for (int i = 0; i < index; i++)
                    node = node.Next;
                return node;
            }
        }

        /// <summary>
        /// Add a new item to the end of the list
        /// </summary>
        /// <param name="item">Item to be added</param>
        public void AddLast(T item)
        {
            // if head is null, then this will be the first item
            if (_head == null)
                AddFirstItem(item);
            else
            {
                var newNode = new Node<T>(item);
                _tail.Next = newNode;
                newNode.Next = _head;
                newNode.Previous = _tail;
                _tail = newNode;
                _head.Previous = _tail;
            }
            ++_count;
        }

        void AddFirstItem(T item)
        {
            _head = new Node<T>(item);
            _tail = _head;
            _head.Next = _tail;
            _head.Previous = _tail;
        }

        /// <summary>
        /// Adds item to the last
        /// </summary>
        /// <param name="item">Item to be added</param>
        public void AddFirst(T item)
        {
            if (_head == null)
                AddFirstItem(item);
            else
            {
                var newNode = new Node<T>(item);
                _head.Previous = newNode;
                newNode.Previous = _tail;
                newNode.Next = _head;
                _tail.Next = newNode;
                _head = newNode;
            }
            ++_count;
        }

        /// <summary>
        /// Adds the specified item after the specified existing node in the list.
        /// </summary>
        /// <param name="node">Existing node after which new item will be inserted</param>
        /// <param name="item">New item to be inserted</param>
        /// <exception cref="ArgumentNullException"><paramref name="node"/> is NULL</exception>
        /// <exception cref="InvalidOperationException"><paramref name="node"/> doesn't belongs to list</exception>
        public void AddAfter(Node<T> node, T item)
        {
            if (node == null)
                throw new ArgumentNullException("node");
            // ensuring the supplied node belongs to this list
            var temp = FindNode(_head, node.Value);
            if (temp != node)
                throw new InvalidOperationException("Node doesn't belongs to this list");

            var newNode = new Node<T>(item) {Next = node.Next};
            node.Next.Previous = newNode;
            newNode.Previous = node;
            node.Next = newNode;

            // if the node adding is tail node, then repointing tail node
            if (node == _tail)
                _tail = newNode;
            ++_count;
        }

        /// <summary>
        /// Adds the new item after the specified existing item in the list.
        /// </summary>
        /// <param name="existingItem">Existing item after which new item will be added</param>
        /// <param name="newItem">New item to be added to the list</param>
        /// <exception cref="ArgumentException"><paramref name="existingItem"/> doesn't exist in the list</exception>
        public void AddAfter(T existingItem, T newItem)
        {
            // finding a node for the existing item
            var node = Find(existingItem);
            if (node == null)
                throw new ArgumentException("existingItem doesn't exist in the list");
            AddAfter(node, newItem);
        }

        /// <summary>
        /// Adds the specified item before the specified existing node in the list.
        /// </summary>
        /// <param name="node">Existing node before which new item will be inserted</param>
        /// <param name="item">New item to be inserted</param>
        /// <exception cref="ArgumentNullException"><paramref name="node"/> is NULL</exception>
        /// <exception cref="InvalidOperationException"><paramref name="node"/> doesn't belongs to list</exception>
        public void AddBefore(Node<T> node, T item)
        {
            if (node == null)
                throw new ArgumentNullException("node");
            // ensuring the supplied node belongs to this list
            var temp = FindNode(_head, node.Value);
            if (temp != node)
                throw new InvalidOperationException("Node doesn't belongs to this list");

            var newNode = new Node<T>(item);
            node.Previous.Next = newNode;
            newNode.Previous = node.Previous;
            newNode.Next = node;
            node.Previous = newNode;

            // if the node adding is head node, then repointing head node
            if (node == _head)
                _head = newNode;
            ++_count;
        }

        /// <summary>
        /// Adds the new item before the specified existing item in the list.
        /// </summary>
        /// <param name="existingItem">Existing item before which new item will be added</param>
        /// <param name="newItem">New item to be added to the list</param>
        /// <exception cref="ArgumentException"><paramref name="existingItem"/> doesn't exist in the list</exception>
        public void AddBefore(T existingItem, T newItem)
        {
            // finding a node for the existing item
            var node = Find(existingItem);
            if (node == null)
                throw new ArgumentException("existingItem doesn't exist in the list");
            AddBefore(node, newItem);
        }

        /// <summary>
        /// Finds the supplied item and returns a node which contains item. Returns NULL if item not found
        /// </summary>
        /// <param name="item">Item to search</param>
        /// <returns><see cref="Node&lt;T&gt;"/> instance or NULL</returns>
        public Node<T> Find(T item)
        {
            var node = FindNode(_head, item);
            return node;
        }

        /// <summary>
        /// Removes the first occurance of the supplied item
        /// </summary>
        /// <param name="item">Item to be removed</param>
        /// <returns>TRUE if removed, else FALSE</returns>
        public bool Remove(T item)
        {
            // finding the first occurance of this item
            var nodeToRemove = Find(item);
            return nodeToRemove != null && RemoveNode(nodeToRemove);
        }

        bool RemoveNode(Node<T> nodeToRemove)
        {
            var previous = nodeToRemove.Previous;
            previous.Next = nodeToRemove.Next;
            nodeToRemove.Next.Previous = nodeToRemove.Previous;

            // if this is head, we need to update the head reference
            if (_head == nodeToRemove)
                _head = nodeToRemove.Next;
            else if (_tail == nodeToRemove)
                _tail = _tail.Previous;

            --_count;
            return true;
        }

        /// <summary>
        /// Removes all occurances of the supplied item
        /// </summary>
        /// <param name="item">Item to be removed</param>
        public void RemoveAll(T item)
        {
            bool removed;
            do
            {
                removed = Remove(item);
            } while (removed);
        }

        /// <summary>
        /// Clears the list
        /// </summary>
        public void Clear()
        {
            _head = null;
            _tail = null;
            _count = 0;
        }

        /// <summary>
        /// Removes head
        /// </summary>
        /// <returns>TRUE if successfully removed, else FALSE</returns>
        public bool RemoveHead()
        {
            return RemoveNode(_head);
        }

        /// <summary>
        /// Removes tail
        /// </summary>
        /// <returns>TRUE if successfully removed, else FALSE</returns>
        public bool RemoveTail()
        {
            return RemoveNode(_tail);
        }

        Node<T> FindNode(Node<T> node, T valueToCompare)
        {
            Node<T> result = null;
            if (_comparer.Equals(node.Value, valueToCompare))
                result = node;
            else if (node.Next != _head)
                result = FindNode(node.Next, valueToCompare);
            return result;
        }

        /// <summary>
        /// Gets a forward enumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            var current = _head;
            if (current == null) yield break;
            do
            {
                yield return current.Value;
                current = current.Next;
            } while (current != _head);
        }

        /// <summary>
        /// Gets a reverse enumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetReverseEnumerator()
        {
            var current = _tail;
            if (current == null) yield break;
            do
            {
                yield return current.Value;
                current = current.Previous;
            } while (current != _tail);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Determines whether a value is in the list.
        /// </summary>
        /// <param name="item">Item to check</param>
        /// <returns>TRUE if item exist, else FALSE</returns>
        public bool Contains(T item)
        {
            return Find(item) != null;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (arrayIndex < 0 || arrayIndex > array.Length)
                throw new ArgumentOutOfRangeException("arrayIndex");

            var node = _head;
            do
            {
                array[arrayIndex++] = node.Value;
                node = node.Next;
            } while (node != _head);
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        bool ICollection<T>.IsReadOnly
        {
            get { return false; }
        }

        void ICollection<T>.Add(T item)
        {
            AddLast(item);
        }
    }
}