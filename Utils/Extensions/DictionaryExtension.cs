using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Utils.Extensions
{
    public static class DictionaryExtension
    {
        #region GetValue

        /// <summary>
        /// <para>Получение значения из словаря.</para>
        /// <para>Eсли такого значения в таблице нет - возвращается значение по умолчанию.</para>
        /// </summary>
        public static object GetValue(this IDictionary T, object key, object defValue)
        {
            return (!T.IsNull() && T.Contains(key) && T[key] != null) ? T[key] : defValue;
        }

        public static object GetValue(this IDictionary T, object key)
        {
            return (!T.IsNull() && T.Contains(key)) ? T[key] : null;
        }

        #endregion

        #region GetString

        public static string GetString(this IDictionary T, object key)
        {
            return GetString(T, key, "");
        }

        public static string GetString(this IDictionary T, object key, string defValue)
        {
            return (!T.IsNull() && T.Contains(key) && T[key] != null) ? T[key].AsString(defValue) : defValue;
        }

        #endregion

        #region GetInt

        public static int GetInt(this IDictionary T, object key)
        {
            return GetInt(T, key, 0);
        }

        public static int GetInt(this IDictionary T, object key, int defValue)
        {
            return (!T.IsNull() && T.Contains(key) && T[key] != null) ? T[key].AsInt(defValue) : defValue;
        }

        #endregion

        #region GetFloat

        public static float GetFloat(this IDictionary T, object key)
        {
            return GetFloat(T, key, 0);
        }

        public static float GetFloat(this IDictionary T, object key, float defValue)
        {
            return (!T.IsNull() && T.Contains(key) && T[key] != null) ? T[key].AsFloat(defValue) : defValue;
        }

        #endregion

        #region GetDouble

        public static double GetDouble(this IDictionary T, object key)
        {
            return GetDouble(T, key, 0);
        }

        public static double GetDouble(this IDictionary T, object key, double defValue)
        {
            return (!T.IsNull() && T.Contains(key) && T[key] != null) ? T[key].AsDouble(defValue) : defValue;
        }

        #endregion

        #region GetDecimal

        public static decimal GetDecimal(this IDictionary T, object key)
        {
            return GetDecimal(T, key, 0);
        }

        public static decimal GetDecimal(this IDictionary T, object key, decimal defValue)
        {
            return (!T.IsNull() && T.Contains(key) && T[key] != null) ? T[key].AsDecimal(defValue) : defValue;
        }

        #endregion

        #region GetBool

        public static bool GetBool(this IDictionary T, object key)
        {
            return GetBool(T, key, false);
        }

        public static bool GetBool(this IDictionary T, object key, bool defValue)
        {
            return (!T.IsNull() && T.Contains(key) && T[key] != null) ? T[key].AsBool(defValue) : defValue;
        }

        #endregion

        #region GetDateTime

        public static DateTime GetDateTime(this IDictionary T, object key)
        {
            return GetDateTime(T, key, DateTime.MinValue);
        }

        public static DateTime GetDateTime(this IDictionary T, object key, DateTime defValue)
        {
            DateTime res = (!T.IsNull() && T.Contains(key) && T[key] != null) ? T[key].AsDateTime(defValue) : defValue;
            return DateTime.SpecifyKind(res, DateTimeKind.Unspecified);
        }

        #endregion

        #region GetDate

        public static DateTime GetDate(this IDictionary T, object key)
        {
            return GetDateTime(T, key, DateTime.MinValue).Date;
        }

        public static DateTime GetDate(this IDictionary T, object key, DateTime defValue)
        {
            return GetDateTime(T, key, defValue).Date;
        }

        #endregion

        #region GetTime

        public static TimeSpan GetTime(this IDictionary T, object key)
        {
            return GetDateTime(T, key, DateTime.MinValue).TimeOfDay;
        }

        public static TimeSpan GetTime(this IDictionary T, object key, DateTime defValue)
        {
            return GetDateTime(T, key, defValue).TimeOfDay;
        }

        #endregion

        #region Get<T>

        /// <summary>
        /// Gets value of specified key from dictionary. If no key contains in dictionary - returns default value
        /// </summary>
        /// <typeparam name="T">Type of result</typeparam>
        /// <param name="d">Dictionary object</param>
        /// <param name="key">Key value</param>
        /// <param name="defValue">Default value</param>
        public static T Get<T>(this IDictionary d, object key, T defValue = default(T))
        {
            return GetValue(d, key, defValue).As(defValue);
        }

        /// <summary>
        /// Gets value of specified key from strong typed dictionary. If no key contains in dictionary - returns default value
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of result</typeparam>
        /// <param name="d">Dictionary object</param>
        /// <param name="key">Key value</param>
        /// <param name="defValue">Default value</param>
        public static TValue Get<TKey, TValue>(this Dictionary<TKey, TValue> d, TKey key,
                                               TValue defValue = default(TValue))
        {
            return GetValue(d, key, defValue).As(defValue);
        }

        /// <summary>
        /// Получить элемент с ключом, ближайшим к данному
        /// </summary>
        /// <typeparam name="TValue">Тип результата</typeparam>
        /// <param name="dictionary">Dictionary object</param>
        /// <param name="key">ключ</param>
        /// <param name="defValue">значение по умолчанию</param>
        /// <returns></returns>
        public static TValue GetNearest<TValue>(this Dictionary<double, TValue> dictionary, double key,
                                                          TValue defValue = default(TValue))
        {
            if (dictionary.Keys.Count == 0)
                return defValue;
            var list = new List<double>(dictionary.Keys);
            var nearestKey = list.OrderBy(v => Math.Abs(v - key)).First();
            return dictionary.Get(nearestKey);
        }

        /// <summary>
        /// Получить элемент с ключом, ближайшим и не-большим данного
        /// </summary>
        /// <typeparam name="TValue">Тип результата</typeparam>
        /// <param name="dictionary">Dictionary object</param>
        /// <param name="key">ключ</param>
        /// <param name="realKey">реальный ключ, при наличии объекта в словаре точно по переданому key, realKey и key совпадают</param>
        /// <param name="defValue">значение по умолчанию</param>
        /// <returns></returns>
        public static TValue GetLowerBoundValue<TValue>(this IDictionary<double, TValue> dictionary, double key, out double realKey,
                                                          TValue defValue = default(TValue))
        {
            realKey = double.NegativeInfinity;
            foreach (var value1 in dictionary.Keys)
            {
                if (value1 <= key && value1 > realKey)
                    realKey = value1;
            }
            if (double.IsNegativeInfinity(realKey))
            {
                realKey = double.NaN;
                return defValue;
            }
            var value = dictionary[realKey];
            return value;
        }

        /// <summary>
        /// Получить элемент с ключом, ближайшим и большим данного
        /// </summary>
        /// <typeparam name="TValue">Тип результата</typeparam>
        /// <param name="dictionary">Dictionary object</param>
        /// <param name="key">ключ</param>
        /// <param name="realKey">реальный ключ, при наличии объекта в словаре точно по переданому key, realKey и key совпадают</param>
        /// <param name="defValue">значение по умолчанию</param>
        /// <returns></returns>
        public static TValue GetHigherBoundValue<TValue>(this IDictionary<double, TValue> dictionary, double key, out double realKey,
                                                          TValue defValue = default(TValue))
        {
            realKey = double.PositiveInfinity;
            foreach (var value1 in dictionary.Keys)
            {
                if (value1 > key && value1 < realKey)
                    realKey = value1;
            }
            if (double.IsPositiveInfinity(realKey))
            {
                realKey = double.NaN;
                return defValue;
            }
            var value = dictionary[realKey];
            return value;
        }

        public static TValue GetLastValue<TValue>(this IDictionary<double, TValue> dictionary, TValue defValue = default(TValue))
        {
            double realKey;
            return dictionary.GetLastValue(out realKey, defValue);
        }

        public static TValue GetLastValue<TValue>(this IDictionary<double, TValue> dictionary, out double realKey, TValue defValue = default(TValue))
        {
            realKey = double.NaN;
            if (!dictionary.Any())
                return defValue;
            realKey = dictionary.Keys.OrderBy(v => v).LastOrDefault();
            return dictionary[realKey];
        }

        #endregion

        #region GetHigherNearest


        #endregion

        public static int RemoveAllKeysHigherThen<TValue>(this IDictionary<double, TValue> dictionary, double threshold)
        {
            var higherKeys = dictionary.Keys.Where(v => v > threshold).ToArray();
            foreach (var key in higherKeys)
            {
                dictionary.Remove(key);
            }
            return higherKeys.Length;
        }

        #region Formatting

        /// <summary>
        /// Преобразует IDictionary в строку параметров вида: key[delim1]value[delim2]
        /// </summary>
        public static string ToParametersString(this IDictionary dictionary, string delim1 = null, string delim2 = null)
        {
            delim1 = delim1 ?? "=";
            delim2 = delim2 ?? ";";
            return dictionary.Keys.Cast<object>().Aggregate("",
                                                            (current, key) =>
                                                            current + (key + delim1 + dictionary[key] + delim2));
        }

        #endregion
    }
} ;