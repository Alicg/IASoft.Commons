using System;
using System.Collections.Generic;

namespace Utils.Comparers
{
    public class ComparerEx<T, TCompareValue> : IEqualityComparer<T>
    {
        private readonly Func<T, TCompareValue> _compareValue;

        public ComparerEx(Func<T, TCompareValue> compareValue)
        {
            _compareValue = compareValue;
        }

        #region Implementation of IEqualityComparer<in T>

        public bool Equals(T x, T y)
        {
            return Equals(_compareValue(x), _compareValue(y));
        }

        public int GetHashCode(T obj)
        {
            return _compareValue(obj).GetHashCode();
        }

        #endregion
    }
}