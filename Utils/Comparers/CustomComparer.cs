using System;
using System.Collections.Generic;

namespace Utils.Comparers
{
    public class CustomComparer<T> : Comparer<T>
    {
        private readonly Func<T, T, int> _compare;

        public CustomComparer(Func<T, T, int> compare)
        {
            _compare = compare;
        }

        #region Overrides of Comparer<T>

        public override int Compare(T x, T y)
        {
            return _compare(x, y);
        }

        #endregion
    }
}