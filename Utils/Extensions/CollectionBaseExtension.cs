using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Utils.Extensions
{
    public static class CollectionBaseExtension
    {
        #region IsCollectionEquals

        /// <summary>
        /// Comparing two collections with item-to-item method
        /// </summary>
        public static bool IsCollectionEquals(this ICollection self, CollectionBase obj)
        {
            bool res = self.Count == obj.Count;
            var l1 = self as IList;
            var l2 = obj as IList;
            for (int i = 0; res && i < self.Count; i++)
                res = l1[i].IsPropsEquals(l2[i]);
            return res;
        }


        public static bool IsCollectionEqualsByFields(this CollectionBase self, CollectionBase obj)
        {
            bool res = self.Count == obj.Count;
            var l1 = self as IList;
            var l2 = obj as IList;
            for (int i = 0; res && i < self.Count; i++)
                res = l1[i].IsFieldsEquals(l2[i]);
            return res;
        }

        public static bool IsCollectionEquals(this CollectionBase self, CollectionBase obj)
        {
            bool res = self.Count == obj.Count;
            var l1 = self as IList;
            var l2 = obj as IList;
            for (int i = 0; res && i < self.Count; i++)
                res = l1[i].IsEquals(l2[i]);
            return res;
        }

        #endregion

/*
        public static string GetUniqueName(this ICollection self, string template, int maxLength = 0)
        {
            return self.GetUniqueName<string>(template, null, maxLength);
        }
*/

        /// <summary>
        /// Removing list items by condition from generic collection passed as this parameter
        /// </summary>
        /// <returns>The same list</returns>
        public static ICollection<T> DeleteUntyped<T>(this ICollection<T> self, Func<T, bool> condition)
        {
            foreach (T item in self.Where(condition).ToArray())
            {
                self.Remove(item);
            }
            return self;
        }
    }
}