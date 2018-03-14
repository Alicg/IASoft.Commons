using System;
using System.Collections.Generic;

namespace Utils.DataStrucures
{
    public class TwoWayDictionary<T, TK>
    {
        private readonly Dictionary<T, TK> firstDictionary;
        private readonly Dictionary<TK, T> secondDictionary;

        public TwoWayDictionary()
        {
            this.firstDictionary = new Dictionary<T, TK>();
            this.secondDictionary = new Dictionary<TK, T>();
        }

        public TK this[T value]
        {
            get
            {
                if (this.firstDictionary.ContainsKey(value))
                {
                    return this.firstDictionary[value];
                }

                throw new ArgumentException(nameof(value));
            }
        }

        public T this[TK value]
        {
            get
            {
                if (this.secondDictionary.ContainsKey(value))
                {
                    return this.secondDictionary[value];
                }

                throw new ArgumentException(nameof(value));
            }
        }

        public IEnumerable<T> AllFirst => this.firstDictionary.Keys;

        public IEnumerable<TK> AllSecond => this.secondDictionary.Keys;

        public void Add(T first, TK second)
        {
            this.firstDictionary.Add(first, second);
            this.secondDictionary.Add(second, first);
        }

        public void Remove(T first)
        {
            var second = this.firstDictionary[first];
            this.firstDictionary.Remove(first);
            this.secondDictionary.Remove(second);
        }

        public void Remove(TK second)
        {
            var first = this.secondDictionary[second];
            this.secondDictionary.Remove(second);
            this.firstDictionary.Remove(first);
        }

        public bool Contains(T first)
        {
            return this.firstDictionary.ContainsKey(first);
        }

        public bool Contains(TK second)
        {
            return this.secondDictionary.ContainsKey(second);
        }
    }  
}