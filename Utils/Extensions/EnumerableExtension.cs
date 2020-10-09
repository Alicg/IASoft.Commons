using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Utils.Comparers;

namespace Utils.Extensions
{
    [DebuggerStepThrough]
    public static class EnumerableExtension
    {
        #region Delete

        /// <summary>
        /// Removing array items by condition 
        /// </summary>
        /// <returns>Result is new List</returns>
        public static List<T> Delete<T>(this T[] self, Func<T, bool> condition)
        {
            var res = new List<T>();
            self.ForEach(i => { if (!condition(i)) res.Add(i); });
            return res;
        }

        /// <summary>
        /// Removing list items by condition from generic list passed as this parameter
        /// </summary>
        /// <returns>The same list</returns>
        public static IList<T> Delete<T>(this IList<T> self, Func<T, bool> condition)
        {
            for (int i = self.Count - 1; i >= 0; i--)
                if (condition(self[i]))
                    self.RemoveAt(i);
            return self;
        }

        /// <summary>
        /// Removing list items by condition from non-generic list passed as this parameter
        /// </summary>
        /// <returns>The same list</returns>
        public static IList DeleteUntyped<T>(this IList self, Func<T, bool> condition)
        {
            foreach (T item in self.Cast<T>().Where(condition).ToArray())
            {
                self.Remove(item);
            }
            return self;
        }

        #endregion

        #region ForEach

        /// <summary>
        /// Enumeration array items
        /// </summary>
        public static T[] ForEach<T>(this T[] self, Action<T> action)
        {
            if (self == null) return null;
            Array.ForEach(self, action);
            return self;
        }

        /// <summary>
        /// Enumeration IEnumerable items 
        /// </summary>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> self, Action<T> action)
        {
            if (self == null) return null;
            foreach (T item in self) action(item);
            return self;
        }

        /// <summary>
        /// Enumeration IEnumerable items of specified type
        /// </summary>
        public static IEnumerable ForEach<T>(this IEnumerable self, Action<T> action)
        {
            if (self == null) return null;
            foreach (T item in self.OfType<T>())
            {
                action(item);
            }
            return self;
        }

/*
        public static IEnumerable ForEach(this IEnumerable self, Action<object> action)
        {
            if (self == null) return null;
            foreach (var item in self) action(item);
            return self;
        }
*/

        #endregion

        #region Find

/*
        public static T Find<T>(this IEnumerable<T> self, Func<T, bool> condition = null)
        {
            return condition == null ? self.FirstOrDefault() : self.FirstOrDefault(condition);
        }
*/

        /// <summary>
        /// Find first element of specified type satisfied given condition. If such element not found returns null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static T Find<T>(this ICollection self, Func<T, bool> condition = null)
        {
            return self.OfType<T>().FirstOrDefault(condition ?? delegate { return true; });
        }

        #endregion

        #region Exists / NotExists

        public static bool Exists<T>(this IEnumerable<T> self, Func<T, bool> condition)
        {
            return self.Where(condition).Any();
        }

        public static bool Exists<T>(this IEnumerable self, Func<T, bool> condition)
        {
            //return (from object item in self where item is T select item).Cast<T>().Exists(condition);
            return self.OfType<T>().Exists(condition);
        }

        public static bool NotExists<T>(this IEnumerable<T> self, Func<T, bool> condition)
        {
            return !self.Exists(condition);
        }

        public static bool NotExists<T>(this IEnumerable self, Func<T, bool> condition)
        {
            return !self.Exists(condition);
        }

        #endregion

        #region Contains

        public static bool Contains<T>(this T[] self, T value)
        {
            return Array.Exists(self, e => e.Equals(value));
        }

        #endregion

        #region IsEmpty / IsNotEmpty

        public static bool IsEmpty<T>(this IList<T> self)
        {
            return self == null || self.Count == 0;
        }

        #endregion

        public static bool ContainsAll<T>(this IEnumerable<T> self, IEnumerable<T> list)
        {
            return self.Count(list.Contains) == self.Count();
        }

        public static string GetUniqueName<T>(this IEnumerable<T> self, string template, int maxLength)
        {
            return self.GetUniqueName(template, null, maxLength);
        }

        public static string GetUniqueName<T>(this IEnumerable<T> self, string template,
                                              Func<T, string> getString = null, int maxLength = 0)
        {
            if (self != null)
            {
                if (getString == null) getString = (i => i.ToString());
                for (int i = 1; i < int.MaxValue; i++)
                {
                    string res = template.Fmt(i);
                    if (res == template) res += i;
                    if (maxLength > 0 && res.Length > maxLength)
                    {
                        template = template.Substring(0, template.Length - res.Length + maxLength);
                        if (template.IsEmpty())
                            break;
                        res = template.Fmt(i);
                        if (res == template) res += i;
                    }
                    if (!self.Any(item => getString(item).SameText(res)))
                        return res;
                }
            }
            throw new Exception("Can't get an unique name");
        }

        public static string GetUniqueNameOfType<T>(this IEnumerable self, string template,
                                                    Func<T, string> getString = null, int maxLength = 0)
        {
            return self.OfType<T>().GetUniqueName(template, getString, maxLength);
        }

        public static IEnumerable<T> Distinct<T, TCompareValue>(this IEnumerable<T> self,
                                                                Func<T, TCompareValue> compareValue)
        {
            return self.Distinct(new ComparerEx<T, TCompareValue>(compareValue));
        }

        public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source, TSource defaultValue,
                                                      Func<TSource, bool> predicate = null)
        {
            if (predicate != null)
                source = source.Where(predicate);
            return source.Any() ? source.First() : defaultValue;
        }

        public static TSource LastOrDefault<TSource>(this IEnumerable<TSource> source, TSource defaultValue,
                                                     Func<TSource, bool> predicate = null)
        {
            if (predicate != null)
                source = source.Where(predicate);
            return source.Any() ? source.First() : defaultValue;
        }

        public static TSource SingleOrDefault<TSource>(this IEnumerable<TSource> source, TSource defaultValue,
                                                       Func<TSource, bool> predicate = null)
        {
            if (predicate != null)
                source = source.Where(predicate);
            return source.Any() ? source.Single() : defaultValue;
        }

        public static TList Clone<TList, TItem>(this TList list)
            where TList : IList<TItem>, new()
            where TItem : class, new()
        {
            var res = new TList();
            foreach (TItem obj in list)
            {
                var newObj = new TItem();
                newObj.CopyFrom(obj);
                res.Add(newObj);
            }
            return res;
        }

        public static BindingList<TSource> ToBindingList<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null) return null;
            var res = new BindingList<TSource>();
            foreach (TSource i in source)
                res.Add(i);
            return res;
        }
        
        public static IEnumerable<T> Traverse<T>(this IEnumerable<T> items, 
            Func<T, IEnumerable<T>> childSelector)
        {
            var stack = new Stack<T>(items);
            while(stack.Any())
            {
                var next = stack.Pop();
                yield return next;
                foreach(var child in childSelector(next))
                    stack.Push(child);
            }
        }
    }
}