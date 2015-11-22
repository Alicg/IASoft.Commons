using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace Utils.Extensions
{
    public static class ObjectExtension
    {
        /// <summary>
        /// Checks whether parameter is null or DBNull. For not-nullable types returns false.
        /// </summary>
        /// <typeparam name="T">Type of parameter</typeparam>
        /// <param name="obj">Checking parameter</param>
        public static bool IsNull<T>(this T obj)
        {
            //if (typeof(T).CanBeNull())
            return (obj == null || (object) obj == DBNull.Value);
            //return false;
        }

        /// <summary>
        /// Checks whether parameter is not null or DBNull. For not-nullable types returns true.
        /// </summary>
        /// <typeparam name="T">Type of parameter</typeparam>
        /// <param name="obj">Checking parameter</param>
        public static bool IsNotNull<T>(this T obj)
        {
            return !IsNull(obj);
        }

/*
        public static bool IsNull(this object obj)
        {
            return (obj == null || obj == DBNull.Value);
        }
        public static bool IsNotNull(this object obj)
        {
            return (obj != null && obj != DBNull.Value);
        }
*/

        internal static string ToFloatString(this object obj)
        {
            string res = obj.ToString();
            return res.Replace(".", NumberFormatInfo.CurrentInfo.NumberDecimalSeparator).Replace(",",
                                                                                                 NumberFormatInfo.
                                                                                                     CurrentInfo.
                                                                                                     NumberDecimalSeparator);
        }

        #region AsFloat

        /// <summary>
        /// Converts object to float. Returns default value for null objects.
        /// </summary>
        /// <param name="obj">Converting object</param>
        /// <param name="defValue">Value that will be return if object is null</param>
        /// <returns>Float representation of object</returns>
        public static float AsFloat(this object obj, float defValue = 0)
        {
            float res = defValue;
            try
            {
                if (!obj.IsNull() && Single.TryParse(obj.ToFloatString(), out defValue))
                    res = defValue;
            }
            catch
            {
            }
            return res;
        }

        #endregion

        #region AsDouble

        /// <summary>
        /// Converts object to double. Returns default value for null objects.
        /// (for old framework versions)
        /// </summary>
        /// <param name="obj">Converting object</param>
        /// <returns>Double representation of object</returns>
        public static double AsDouble(this object obj)
        {
            return obj.AsDouble(0);
        }

        /// <summary>
        /// Converts object to double. Returns default value for null objects.
        /// </summary>
        /// <param name="obj">Converting object</param>
        /// <param name="defValue">Value that will be return if object is null</param>
        /// <returns>Double representation of object</returns>
        public static double AsDouble(this object obj, double defValue = 0)
        {
            double res = defValue;
            try
            {
                if (!obj.IsNull() && Double.TryParse(obj.ToFloatString(), out defValue))
                    res = defValue;
            }
            catch
            {
            }
            return res;
        }

        #endregion

        #region AsDecimal

        /// <summary>
        /// Converts object to decimal. Returns default value for null objects.
        /// </summary>
        /// <param name="obj">Converting object</param>
        /// <param name="defValue">Value that will be return if object is null</param>
        /// <returns>Decimal representation of object</returns>
        public static decimal AsDecimal(this object obj, decimal defValue = 0)
        {
            decimal res = defValue;
            try
            {
                if (!obj.IsNull() && Decimal.TryParse(obj.ToFloatString(), out defValue))
                    res = defValue;
            }
            catch
            {
            }
            return res;
        }

        #endregion

        #region AsBool

        /// <summary>
        /// Converts object to boolean. Returns default value for null objects.
        /// (for old framework versions)
        /// </summary>
        /// <param name="obj">Converting object</param>
        /// <returns>Boolean representation of object</returns>
        public static bool AsBool(this object obj)
        {
            return obj.AsBool(false);
        }

        /// <summary>
        /// Converts object to boolean. Returns default value for null objects.
        /// </summary>
        /// <param name="obj">Converting object</param>
        /// <param name="defValue">Value that will be return if object is null</param>
        /// <returns>Boolean representation of object</returns>
        public static bool AsBool(this object obj, bool defValue = false)
        {
            bool res = defValue;
            try
            {
                if (!obj.IsNull())
                    if (obj.AsString() == "0" || obj.AsString().SameText("false") || obj.AsString() == "")
                        res = false;
                    else
                        res = true;
            }
            catch
            {
            }
            return res;
        }

        #endregion

        #region AsDateTime

        /// <summary>
        /// Converts object to date and time. Returns default value for null objects. 
        /// Date time kind is set to DateTimeKind.Unspecified to be time zone undepended.
        /// </summary>
        /// <param name="obj">Converting object</param>
        /// <param name="defValue">Value that will be return if object is null</param>
        /// <returns>Time zone undepended DateTime representation of object.</returns>
        public static DateTime AsDateTime(this object obj, DateTime defValue = default(DateTime))
        {
            DateTime res = defValue;
            try
            {
                if (obj is DateTime) res = (DateTime) obj;
                else if (!obj.IsNull() && DateTime.TryParse(obj.ToString(), out defValue))
                    res = defValue;
            }
            catch
            {
            }
            return DateTime.SpecifyKind(res, DateTimeKind.Unspecified);
        }

        #endregion

        #region AsDateTimeWithoutMs

        public static DateTime AsDateTimeWithoutMs(this object obj, DateTime defValue = default(DateTime))
        {
            DateTime res = defValue;
            try
            {
                if (!obj.IsNull() && DateTime.TryParse(obj.ToString(), out defValue))
                    res = defValue;
            }
            catch
            {
            }
            return DateTime.SpecifyKind(res.AddMilliseconds(-res.Millisecond), DateTimeKind.Unspecified);
        }

        #endregion

        #region AsDate

        /// <summary>
        /// Converts date with time to date without time.
        /// If time less than 12:00 - returns only date without time, otherwise returns date + 1 day without time.
        /// Date time kind is set to DateTimeKind.Unspecified to be time zone undepended.
        /// </summary>
        /// <returns>Time zone undepended date representation of DateTime.</returns>
        public static DateTime RoundDate(this DateTime obj)
        {
            DateTime resDate = obj.Hour < 12 ? obj.Date : obj.Date.AddDays(1);
            resDate = DateTime.SpecifyKind(resDate, DateTimeKind.Unspecified);
            return resDate;
        }

        /// <summary>
        /// Converts object to date without time. Returns default value for null objects.
        /// If time less than 12:00 - returns only date without time, otherwise returns date + 1 day without time.
        /// Date time kind is set to DateTimeKind.Unspecified to be time zone undepended.
        /// </summary>
        /// <param name="obj">Converting object</param>
        /// <param name="defValue">Value that will be return if object is null</param>
        /// <returns>Time zone undepended date representation of object.</returns>
        public static DateTime AsDate(this object obj, DateTime defValue = default(DateTime))
        {
            DateTime res = defValue.Date;
            try
            {
                if (!obj.IsNull() && DateTime.TryParse(obj.ToString(), out defValue))
                    res = defValue.Date;
            }
            catch
            {
            }
            return DateTime.SpecifyKind(res, DateTimeKind.Unspecified);
        }

        public static DateTime AsDateIfEmpty(this object obj, DateTime defValue = default(DateTime))
        {
            DateTime res = defValue.Date;
            try
            {
                DateTime parse;
                if (!obj.IsNull() && DateTime.TryParse(obj.ToString(), out parse) && parse != default(DateTime))
                    res = parse.Date;
            }
            catch
            {
            }
            return DateTime.SpecifyKind(res, DateTimeKind.Unspecified);
        }

        public static DateTime? AsDateOrNull(this object obj)
        {
            return obj.IsNotNull() ? obj.AsDate() : (DateTime?) null;
        }

        #endregion

        #region AsTime

        public static TimeSpan AsTime(this object obj, DateTime defValue = default(DateTime))
        {
            TimeSpan res = defValue.TimeOfDay;
            try
            {
                if (obj is DateTime) res = ((DateTime) obj).TimeOfDay;
                else if (!obj.IsNull() && DateTime.TryParse(obj.ToString(), out defValue))
                    res = defValue.TimeOfDay;
            }
            catch
            {
            }
            return res;
        }

        #endregion

        #region AsSql

        public static string AsSql(this object obj, string defValue = "Null")
        {
            if (obj.IsNull()) return defValue;
            if (obj is string) return obj.ToString().QuotedStr();
            // BACKTRACK specific behaviour only
            if (obj is byte[]) return obj.AsString().QuotedStr();

            if (obj is bool) return obj.AsBool() ? "1" : "0";
            return obj is DateTime
                       ? obj.AsDate().ToString("dd.MM.yyyy").QuotedStr()
                       : obj.AsString().Replace(',', '.');
        }

        #endregion

        #region As

        /// <summary>
        /// Converts object to specified type. Returns default value for null objects.
        /// </summary>
        /// <typeparam name="T">Type of result</typeparam>
        /// <param name="obj">Converting object</param>
        /// <param name="defValue">Value that will be return if object is null</param>
        public static T As<T>(this object obj, T defValue = default(T))
        {
            T res = defValue;
            try
            {
                if (!obj.IsNull())
                {
                    if (typeof (T) == typeof (string)) res = (T) (object) obj.AsString();
                    else if (typeof (T) == typeof (int) || typeof (T) == typeof (int?))
                        res = (T) (object) obj.AsInt(defValue.AsInt());
                    else if (typeof (T) == typeof (double) || typeof (T) == typeof (double?))
                        res = (T) (object) obj.AsDouble(defValue.AsDouble());
                    else if (typeof (T) == typeof (decimal) || typeof (T) == typeof (decimal?))
                        res = (T) (object) obj.AsDecimal(defValue.AsDecimal());
                    else if (typeof (T) == typeof (bool) || typeof (T) == typeof (bool?))
                        res = (T) (object) obj.AsBool(defValue.AsBool());
                    else if (typeof (T) == typeof (DateTime) || typeof (T) == typeof (DateTime?))
                        res = (T) (object) obj.AsDateTime(defValue.AsDateTime());
                    else if (typeof (T) == typeof (float) || typeof (T) == typeof (float?))
                        res = (T) (object) obj.AsFloat(defValue.AsFloat());
                    else if ((typeof (T) == typeof (char) || typeof (T) == typeof (char?)) &&
                             obj is string)
                    {
                        if (obj.AsString().IsNotEmpty(true))
                            res = (T) (object) (obj.AsString()[0]);
                    }
                    else if ((typeof (T) == typeof (char) || typeof (T) == typeof (char?)) &&
                             obj != null && obj.GetType().IsEnum)
                    {
                        // enum -> char
                        res = (T) (object) Convert.ToChar((int) obj);
                    }
                    else if ((typeof (T) == typeof (char) || typeof (T) == typeof (char?)) &&
                             obj != null && obj.GetType().IsInteger())
                    {
                        // int -> char
                        res = (T) (object) Convert.ToChar(obj);
                    }
                    else if (typeof (T).IsEnum)
                    {
                        if (obj is char)
                            res = (T) Enum.ToObject(typeof (T), obj.As<char>());
                        else if (obj is string)
                        {
                            if (obj.AsString().Length == 1)
                                res =
                                    (T) Enum.ToObject(typeof (T), obj.AsString()[0]);
                            else if (obj.AsString().IsNotEmpty())
                                res = (T) Enum.Parse(typeof (T), obj.AsString());
                        }
                        else
                            res = (T) Enum.ToObject(typeof (T), obj.AsInt());
                    }
                    else
                    {
                        res = (T) obj;
                    }
                }
            }
            catch
            {
            }
            return res;
        }

        /// <summary>
        /// Converts object to specified type. Returns default value for null objects.
        /// </summary>
        /// <param name="obj">Converting object</param>
        /// <param name="type">Type of result</param>
        /// <param name="defValue">Value that will be return if object is null</param>
        public static object AsType(this object obj, Type type, object defValue = null)
        {
            object res = defValue ?? type.DefaultValue();
            try
            {
                if (!obj.IsNull())
                {
                    if (type == typeof (string)) res = obj.AsString();
                    else if (type == typeof (int) || type == typeof (int?)) res = obj.AsInt(defValue.AsInt());
                    else if (type == typeof (double) || type == typeof (double?))
                        res = obj.AsDouble(defValue.AsDouble());
                    else if (type == typeof (decimal) || type == typeof (decimal?))
                        res = obj.AsDecimal(defValue.AsDecimal());
                    else if (type == typeof (bool) || type == typeof (bool?))
                        res = obj.AsBool(defValue.AsBool());
                    else if (type == typeof (DateTime) || type == typeof (DateTime?))
                        res = obj.AsDateTime(defValue.AsDateTime());
                    else if (type == typeof (float) || type == typeof (float?))
                        res = obj.AsFloat(defValue.AsFloat());
                    else if ((type == typeof (char) || type == typeof (char?)) &&
                             obj.GetType() == typeof (string)) res = obj.AsString()[0];
                    else if (type.IsEnum)
                    {
                        if (obj is char)
                            res = Enum.ToObject(typeof (char), obj.As<char>());
                        if (obj.GetType() == typeof (string))
                        {
                            res = obj.AsString().Length == 1
                                      ? Enum.ToObject(typeof (char), obj.AsString()[0])
                                      : Enum.Parse(type, obj.AsString());
                        }
                        else
                            res = Enum.ToObject(type, obj.AsInt());
                    }
                    else
                    {
                        res = obj;
                    }
                }
            }
            catch
            {
            }
            return res;
        }

        #endregion

        #region In / NotIn

        /// <summary>
        /// Checks whether the value in spesified set
        /// </summary>
        /// <typeparam name="T">Type of value and set</typeparam>
        /// <param name="value">Checking value</param>
        /// <param name="list">Checking set</param>
        /// <returns>Returns true if value in set and false otherwise</returns>
        public static bool In<T>(this T self, params T[] list)
        {
            return list != null && list.Any(i => Equals(i, self));
        }

        /// <summary>
        /// Checks whether the value not in spesified set
        /// </summary>
        /// <typeparam name="T">Type of value and set</typeparam>
        /// <param name="value">Checking value</param>
        /// <param name="list">Checking set</param>
        /// <returns>Returns true if value not in set and false otherwise</returns>
        public static bool NotIn<T>(this T self, params T[] list)
        {
            return !self.In(list);
        }

        /// <summary>
        /// Checks whether the value in spesified set
        /// </summary>
        /// <typeparam name="T">Type of value and set</typeparam>
        /// <param name="value">Checking value</param>
        /// <param name="list">Checking set</param>
        /// <returns>Returns true if value in set and false otherwise</returns>
        public static bool In<T>(this T self, IEnumerable<T> list)
        {
            return list != null && list.Any(i => Equals(i, self));
        }

        /// <summary>
        /// Checks whether the value not in spesified set
        /// </summary>
        /// <typeparam name="T">Type of value and set</typeparam>
        /// <param name="value">Checking value</param>
        /// <param name="list">Checking set</param>
        /// <returns>Returns true if value not in set and false otherwise</returns>
        public static bool NotIn<T>(this T self, IEnumerable<T> list)
        {
            return !self.In(list);
        }

        #endregion

        #region Limit / LimitMin / LimitMax

        /// <summary>
        /// Limits specified value with minimum and maximum values. 
        /// Returns <paramref name="minValue"/> if source value less than <paramref name="minValue"/>.
        /// Returns <paramref name="maxValue"/> if source value greater than <paramref name="maxValue"/>.
        /// </summary>
        /// <param name="self">Operationg value</param>
        /// <param name="minValue">Minimum value</param>
        /// <param name="maxValue">Maximum value</param>
        public static int Limit(this int self, int minValue, int maxValue)
        {
            return Math.Min(Math.Max(self, minValue), maxValue);
        }

        /// <summary>
        /// Limits specified value with minimum value. 
        /// Returns <paramref name="minValue"/> if source value less than <paramref name="minValue"/>.
        /// </summary>
        /// <param name="self">Operationg value</param>
        /// <param name="minValue">Minimum value</param>
        public static int LimitMin(this int self, int minValue)
        {
            return Math.Max(self, minValue);
        }

        /// <summary>
        /// Limits specified value with maximum values. 
        /// Returns <paramref name="maxValue"/> if source value greater than <paramref name="maxValue"/>.
        /// </summary>
        /// <param name="self">Operationg value</param>
        /// <param name="maxValue">Maximum value</param>
        public static int LimitMax(this int self, int maxValue)
        {
            return Math.Min(self, maxValue);
        }

        public static double Limit(this double self, double minValue, double maxValue)
        {
            return Math.Min(Math.Max(self, minValue), maxValue);
        }

        public static double LimitMin(this double self, double minValue)
        {
            return Math.Max(self, minValue);
        }

        public static double LimitMax(this double self, double maxValue)
        {
            return Math.Min(self, maxValue);
        }

        public static decimal Limit(this decimal self, decimal minValue, decimal maxValue)
        {
            return Math.Min(Math.Max(self, minValue), maxValue);
        }

        public static decimal LimitMin(this decimal self, decimal minValue)
        {
            return Math.Max(self, minValue);
        }

        public static decimal LimitMax(this decimal self, decimal maxValue)
        {
            return Math.Min(self, maxValue);
        }

        public static float Limit(this float self, float minValue, float maxValue)
        {
            return Math.Min(Math.Max(self, minValue), maxValue);
        }

        public static float LimitMin(this float self, float minValue)
        {
            return Math.Max(self, minValue);
        }

        public static float LimitMax(this float self, float maxValue)
        {
            return Math.Min(self, maxValue);
        }

        public static DateTime Limit(this DateTime self, DateTime minValue, DateTime maxValue)
        {
            return self.LimitMin(minValue).LimitMax(maxValue);
        }

        public static DateTime LimitMin(this DateTime self, DateTime minValue)
        {
            return self < minValue ? minValue : self;
        }

        public static DateTime LimitMax(this DateTime self, DateTime maxValue)
        {
            return self > maxValue ? maxValue : self;
        }

        #endregion

        #region IsPropsEquals

        private static bool IsTypeSimple(this Type t1)
        {
            return t1.IsPrimitive
                   || !t1.IsClass
                   || t1.IsEnum
                   || t1 == typeof (String)
                   || t1 == typeof (DateTime)
                   || (t1.FullName != null && t1.FullName.Contains("System.Nullable`1[["));
        }

        /// <summary>
        /// <para>Сравнение двух объектов, путем сравнения значений каждого свойства объета.</para>
        /// <para>Проверяются только к public свойства.</para>
        /// </summary>
        /// <author>Lion</author>
        public static bool IsPropsEquals(this object source, object target, bool ignoreNull = false)
        {
            if (source == null && target == null) return true;
            if (source == null || target == null) return false;

            bool res = Equals(source, target);
            if (!res)
            {
                Type t1 = source.GetType();
                Type t2 = target.GetType();

                if (t1.IsTypeSimple() || t2.IsTypeSimple()) return false;

                PropertyInfo[] pl1 = t1.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                PropertyInfo[] pl2 = t2.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                if (pl1.Length > 0)
                {
                    res = true;
                    object v1;
                    foreach (PropertyInfo p1 in pl1)
                    {
                        //if (
                        //    /*((p1.PropertyType.IsPrimitive || !p1.PropertyType.IsClass)
                        //     || p1.PropertyType.IsEnum
                        //     || p1.PropertyType == typeof(String)
                        //     || p1.PropertyType == typeof(DateTime)
                        //     || (p1.PropertyType.FullName != null && p1.PropertyType.FullName.Contains("System.Nullable`1[[")))*/
                        //     p1.PropertyType.IsTypeSimple()
                        //     && p1.GetIndexParameters().Length == 0 // non-indexed property
                        //)
                        //{
                        v1 = p1.GetValue(source, null);
                        if (ignoreNull && v1 == null) continue;
                        object v2;
                        if (t1 == t2)
                            v2 = p1.GetValue(target, null);
                        else
                        {
                            v2 = v1;
                            foreach (PropertyInfo p2 in pl2)
                            {
                                if (p2.Name != p1.Name) continue;
                                v2 = p2.GetValue(target, null);
                                break;
                            }
                        }
                        res = v1.IsPropsEquals(v2);
                        if (!res)
                            break;
                        //}
                    }
                }
            }
            return res;
        }

        #endregion

        #region IsFieldsEquals

        /// <summary>
        /// <para>Сравнение двух объектов, путем сравнения значений каждого поля объета.</para>
        /// <para>Проверяются только к public поля.</para>
        /// </summary>
        /// <author>Lion</author>
        public static bool IsFieldsEquals(this object source, object target, bool ignoreNull = false)
        {
            if (source == null && target == null) return true;
            if (source == null || target == null) return false;
            bool res = Equals(source, target);
            if (!res)
            {
                res = true;
                Type t1 = source.GetType();
                Type t2 = target.GetType();
                FieldInfo[] pl1 = t1.GetFields(BindingFlags.Instance | BindingFlags.Public);
                FieldInfo[] pl2 = t2.GetFields(BindingFlags.Instance | BindingFlags.Public);
                object v1;
                foreach (FieldInfo p1 in pl1)
                {
                    v1 = p1.GetValue(source);
                    if (ignoreNull && v1 == null) continue;
                    object v2;
                    if (t1 == t2)
                        v2 = p1.GetValue(target);
                    else
                    {
                        v2 = v1;
                        foreach (FieldInfo p2 in pl2)
                        {
                            if (p2.Name != p1.Name) continue;
                            v2 = p2.GetValue(target);
                            break;
                        }
                    }
                    res = v1.IsFieldsEquals(v2);
                    if (!res) break;
                }
            }
            return res;
        }

        #endregion

        #region IsEquals

        /// <summary>
        /// Compare object with defined object 
        /// </summary>
        /// <param name="self">First object</param>
        /// <param name="obj">Second object</param>
        /// <returns>True if objects is equals</returns>
        public static bool IsEquals(this object self, object obj)
        {
            return Equals(self, obj);
        }

        /// <summary>
        /// <para>Сравнение двух объектов, путем сравнения значений каждого поля и свойства объета.</para>
        /// <para>Проверяются только к public поля и свойства.</para>
        /// </summary>
        public static bool IsEqualsData(this object self, object obj, bool ignoreNull = false)
        {
            if (self == obj) return true;
            if (self == null || obj == null) return false;
            return (self.IsFieldsEquals(obj, ignoreNull) && self.IsPropsEquals(obj, ignoreNull));
        }

        #endregion

        #region ExtFields

        /// <summary>
        /// <para>(c) LionSoft</para>
        /// <para>Получить ссылку на поле объекта по имени.</para>
        /// <para>Если такого поля нет - возвращает null.</para>
        /// </summary>
        /// <author>LionSoft</author>
        private static FieldInfo FieldByName(this object self, string fieldName, bool includeNonPublic = false)
        {
            FieldInfo res = null;
            if (self != null && fieldName != "")
            {
                Type typ = self.GetType();
                while (typ != null && res == null)
                {
                    res = typ.GetField(fieldName,
                                       BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
                                       (includeNonPublic ? BindingFlags.NonPublic : BindingFlags.Default));
                    typ = typ.BaseType;
                }
            }
            return res;
        }

        /// <summary>
        /// <para>(c) LionSoft</para>
        /// <para>Get value of object field by its name.</para>
        /// <para>If field doesn't exists - return null.</para>
        /// </summary>
        /// <author>LionSoft</author>
        public static object GetFieldValue(this object self, string fieldName, bool includeNonPublic = false)
        {
            FieldInfo pi = self.FieldByName(fieldName, includeNonPublic);
            return pi != null ? pi.GetValue(self) : null;
        }

        /// <summary>
        /// <para>(c) LionSoft</para>
        /// <para>Set value of object public field by its name.</para>
        /// <para>If field doesn't exists or readonly - do nothing.</para>
        /// <para>Return previous field value.</para>
        /// </summary>
        /// <param name="self">Working object</param>
        /// <param name="fieldName">Field name</param>
        /// <param name="fieldValue">Field value</param>
        /// <param name="includeNonPublic">If true method can change non public fields.</param>
        /// <returns>Return previous field value.</returns>
        /// <author>LionSoft</author>
        public static object SetFieldValue(this object self, string fieldName, object fieldValue,
                                           bool includeNonPublic = false)
        {
            FieldInfo pi = self.FieldByName(fieldName, includeNonPublic);
            object res = null;
            if (pi != null)
            {
                res = pi.GetValue(self);
                pi.SetValue(self, fieldValue);
            }
            return res;
        }

        #endregion

        #region CopyFields

        /// <summary>
        /// <para>Копирование значений полей с одинаковыми именами в другой объект.</para>
        /// <para>Копируются только public поля.</para>
        /// </summary>
        public static T CopyFieldsTo<T>(this object source, T target, bool ignoreNulls = false,
                                        bool onlySimpleTypes = true, List<string> ignoreList = null) where T : class
        {
            if (source == null || target == null) return target;
            Type t1 = source.GetType();
            Type t2 = target.GetType();
            FieldInfo[] pl1 = t1.GetFields(BindingFlags.Instance | BindingFlags.Public);
            FieldInfo[] pl2 = t2.GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (FieldInfo p1 in pl1)
            {
                if (ignoreList != null && ignoreList.Contains(p1.Name)) continue;
                //if (p1.FieldType.IsPrimitive || !p1.FieldType.IsClass || p1.FieldType == typeof(String) || p1.FieldType == typeof(DateTime))
                if (!onlySimpleTypes || p1.FieldType.IsTypeSimple())
                {
                    object v = p1.GetValue(source);
                    if (ignoreNulls && v.IsNull()) continue;
                    if (t1 == t2)
                        p1.SetValue(target, v);
                    else
                        foreach (FieldInfo p2 in pl2)
                        {
                            if (p2.Name == p1.Name)
                            {
                                p2.SetValue(target, v);
                                break;
                            }
                        }
                }
            }
            return target;
        }

        /// <summary>
        /// <para>Копирование значений полей с одинаковыми именами из другого объекта.</para>
        /// <para>Копируются только public поля.</para>
        /// </summary>
        public static T CopyFieldsFrom<T>(this T target, object source, bool ignoreNulls = false,
                                          bool onlySimpleTypes = false) where T : class
        {
            return source.CopyFieldsTo(target, ignoreNulls, onlySimpleTypes);
        }

        #endregion

        #region InvokeByName

        /// <summary>
        /// <para>Вызывает метод объекта по имени и возвращает результат.</para>
        /// <para>Если такого метода нет - возвращает null.</para>
        /// </summary>
        /// <author>Tamersoul</author>
        public static object InvokeByName(this object self, string methodName, object[] args = null)
        {
            object res = null;
            if (self != null && methodName != "")
            {
                res = self.GetType().InvokeMember(methodName, BindingFlags.InvokeMethod, null, self, args);
            }
            return res;
        }

        /// <summary>
        /// <para>Вызывает метод объекта по имени и возвращает результат указанного типа.</para>
        /// <para>Если такого метода нет - возвращает default(тип).</para>
        /// </summary>
        /// <author>Tamersoul</author>
        public static T InvokeByName<T>(this object self, string methodName, object[] args = null)
        {
            T res = default(T);
            if (self != null && methodName != "")
            {
                res = (T) self.GetType().InvokeMember(methodName, BindingFlags.InvokeMethod, null, self, args);
            }
            return res;
        }

        #endregion

        #region ExtProps

        /// <summary>
        /// <para>(c) LionSoft</para>
        /// <para>Получить ссылку на public свойство объекта по имени.</para>
        /// <para>Если такого свойства нет - возвращает null.</para>
        /// </summary>
        /// <author>LionSoft</author>
        internal static PropertyInfo PropByName(this object self, string propName, bool includeNonPublic = false)
        {
            PropertyInfo res = null;
            if (self != null && propName != "")
                res = self.GetType().GetProperty(propName,
                                                 BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
                                                 (includeNonPublic ? BindingFlags.NonPublic : BindingFlags.Default));
            return res;
        }

        /// <summary>
        /// <para>(c) LionSoft</para>
        /// <para>Get value of object property by its name.</para>
        /// <para>If property doesn't exists or writeonly - return null.</para>
        /// </summary>
        /// <author>LionSoft</author>
        public static object GetPropValue(this object self, string propName, bool includeNonPublic = false)
        {
            if (self == null) return null;
            PropertyInfo pi = self.PropByName(propName, includeNonPublic);
            return pi != null && pi.CanRead ? pi.GetValue(self, null) : null;
        }

        /// <summary>
        /// <para>(c) LionSoft</para>
        /// <para>Set value of object public property by its name.</para>
        /// <para>If property doesn't exists or readonly - do nothing.</para>
        /// <para>Return previous property value if can.</para>
        /// </summary>
        /// <param name="self">Working object</param>
        /// <param name="propName">Property name</param>
        /// <param name="propValue">Property value</param>
        /// <param name="forcedWrite">If false - value sets only if properties has different values</param>
        /// <param name="includeNonPublic">If true method can change non public properties.</param>
        /// <returns>Return previous value of property if can.</returns>
        /// <author>LionSoft</author>
        public static object SetPropValue(this object self, string propName, object propValue, bool forcedWrite = false,
                                          bool includeNonPublic = false)
        {
            if (self == null) return null;
            PropertyInfo pi = self.PropByName(propName, includeNonPublic);
            object res = null;
            if (pi != null)
            {
                if (pi.CanRead)
                {
                    res = pi.GetValue(self, null);
                    if (!forcedWrite && res.IsEquals(propValue))
                        return res;
                }
                if (pi.CanWrite)
                    pi.SetValue(self, propValue, null);
            }
            return res;
        }

        #endregion

        #region CopyProps

        /// <summary>
        /// <para>Копирование значений свойств с одинаковыми именами в другой объект.</para>
        /// <para>Копируются только public свойства.</para>
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="target">Target object</param>
        /// <param name="ignoreNulls">If true - ignore null values</param>
        /// <param name="forcedCopy">If false - value copies only if properties has different values</param>
        /// <param name="useXmlIgnoreAttribute">If true - properties with XmlIgnore attribute will be skipped</param>
        /// <param name="skipAttributeNames">List of the attributes name which copying will be skipped</param>
        /// <param name="onlySimpleTypes">Copy only simple-type attributes</param>
        /// <param name="ignoreList">Ignored properties</param>
        public static T CopyPropsTo<T>(this object source, T target, bool ignoreNulls = false, bool forcedCopy = false,
                                       bool useXmlIgnoreAttribute = false, string[] skipAttributeNames = null,
                                       bool onlySimpleTypes = true, List<string> ignoreList = null) where T : class
        {
            if (source == null || target == null) return target;
            Type t1 = target.GetType();
            Type t2 = source.GetType();
            PropertyInfo[] pl1 = t1.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            PropertyInfo[] pl2 = t2.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (PropertyInfo p2 in pl2)
            {
                if (ignoreList != null && ignoreList.Contains(p2.Name)) continue;

                if ( /*  ((p2.PropertyType.IsPrimitive || !p2.PropertyType.IsClass)
                    || p2.PropertyType.IsEnum
                    || p2.PropertyType == typeof(String)
                    || p2.PropertyType == typeof(DateTime)
                    || (p2.PropertyType.FullName != null && p2.PropertyType.FullName.Contains("System.Nullable`1[[")))*/
                    !onlySimpleTypes ||
                    p2.PropertyType.IsTypeSimple() &&
                    p2.GetIndexParameters().Length == 0 // non-indexed property
                    )

                {
                    if (skipAttributeNames != null && skipAttributeNames.Contains(p2.Name))
                        continue;
                    if (!p2.CanWrite || !p2.CanRead)
                        continue;
                    if (useXmlIgnoreAttribute && p2.GetCustomAttribute<XmlIgnoreAttribute>() != null)
                        continue;
                    object v2 = p2.GetValue(source, null);
                    if (ignoreNulls && v2.IsNull()) continue;
                    foreach (PropertyInfo p1 in pl1)
                    {
                        if (p1.Name != p2.Name) continue;
                        if (p1.CanRead)
                        {
                            object v1 = p1.GetValue(target, null);
                            if (forcedCopy || !v1.IsEquals(v2))
                                p1.SetValue(target, v2, null);
                        }
                        break;
                    }
                }
            }
            return target;
        }


        /// <summary>
        /// <para>Копирование значений свойств с одинаковыми именами из другого объекта.</para>
        /// <para>Копируются только public свойства.</para>
        /// </summary>
        /// <param name="target">Target object</param>
        /// <param name="source">Source object</param>
        /// <param name="ignoreNulls">If true - ignore null values</param>
        /// <param name="forcedCopy">If false - value copies only if properties has different values</param>
        /// <param name="useXmlIgnoreAttribute">If true - properties with XmlIgnore attribute will be skipped</param>
        /// <param name="skipAttributeNames">List of the attributes name which copying will be skipped</param>
        public static T CopyPropsFrom<T>(this T target, object source, bool ignoreNulls = false, bool forcedCopy = false,
                                         bool useXmlIgnoreAttribute = false, string[] skipAttributeNames = null,
                                         bool onlySimpleTypes = false) where T : class
        {
            return source.CopyPropsTo(target, ignoreNulls, forcedCopy, useXmlIgnoreAttribute, skipAttributeNames,
                                      onlySimpleTypes);
        }

        #endregion

        #region CopyAll

        /// <summary>
        /// <para>Копирование значений полей и свойств с одинаковыми именами в другой объект.</para>
        /// <para>Копируются только public поля и свойства.</para>
        /// </summary>
        public static T CopyAllTo<T>(this object source, T target, bool ignoreNulls = false, string ignoreList = null)
            where T : class
        {
            source.CopyFieldsTo(target, ignoreNulls, false, ignoreList == null ? null : ignoreList.Split(',').ToList());
            source.CopyPropsTo(target, ignoreNulls, false, false, null, false,
                               ignoreList == null ? null : ignoreList.Split(',').ToList());
            return target;
        }

        /// <summary>
        /// <para>Копирование значений полей и свойств с одинаковыми именами в другой объект.</para>
        /// <para>Копируются только public поля и свойства.</para>
        /// </summary>
        public static T CopyAllTo<T>(this object self, bool ignoreNulls = false, string ignoreList = null)
            where T : class, new()
        {
            var obj = new T();
            self.CopyFieldsTo(obj, ignoreNulls, false, ignoreList == null ? null : ignoreList.Split(',').ToList());
            self.CopyPropsTo(obj, ignoreNulls, false, false, null, false,
                             ignoreList == null ? null : ignoreList.Split(',').ToList());
            return obj;
        }

        /// <summary>
        /// <para>Копирование значений простых свойств с одинаковыми именами из другого объекта.</para>
        /// <para>Копируются только к public поля.</para>
        /// </summary>
        public static T CopyAllFrom<T>(this T self, object obj, bool ignoreNulls = false, string ignoreList = null)
            where T : class
        {
            obj.CopyFieldsTo(self, ignoreNulls, false, ignoreList == null ? null : ignoreList.Split(',').ToList());
            obj.CopyPropsTo(self, ignoreNulls, false, false, null, false,
                            ignoreList == null ? null : ignoreList.Split(',').ToList());
            return self;
        }

        #endregion

        #region Copy

        /// <summary>
        /// <para>(c) LionSoft</para>
        /// <para>Копирование значений простых полей и свойств с одинаковыми именами в другой объект.</para>
        /// <para>Копируются только public поля и свойства.</para>
        /// </summary>
        /// <author>Lion</author>
        public static T CopyTo<T>(this object source, T target, bool ignoreNulls = false, List<string> ignoreList = null)
            where T : class
        {
            source.CopyFieldsTo(target, ignoreNulls, true, ignoreList);
            source.CopyPropsTo(target, ignoreNulls, false, false, null, true, ignoreList);
            return target;
        }

        /// <summary>
        /// <para>(c) LionSoft</para>
        /// <para>Копирование значений простых полей и свойств с одинаковыми именами в другой объект.</para>
        /// <para>Копируются только public поля и свойства.</para>
        /// </summary>
        /// <author>Lion</author>
        public static T CopyTo<T>(this object self, bool ignoreNulls = false, List<string> ignoreList = null)
            where T : class, new()
        {
            var obj = new T();
            self.CopyFieldsTo(obj, ignoreNulls, true, ignoreList);
            self.CopyPropsTo(obj, ignoreNulls, false, false, null, true, ignoreList);
            return obj;
        }

        /// <summary>
        /// <para>(c) LionSoft</para>
        /// <para>Копирование значений простых свойств с одинаковыми именами из другого объекта.</para>
        /// <para>Копируются только к public поля.</para>
        /// </summary>
        /// <author>Lion</author>
        public static T CopyFrom<T>(this T self, object obj, bool ignoreNulls = false) where T : class
        {
            obj.CopyFieldsTo(self, ignoreNulls);
            obj.CopyPropsTo(self, ignoreNulls);
            return self;
        }

        #endregion

        #region SameText

        /// <summary>
        /// Сравнивает два объекта как строки без учета регистра
        /// </summary>
        /// <param name="self"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <author>Lion</author>
        public static bool SameText(this object self, object obj)
        {
            return self.AsString().Equals(obj.AsString(), StringComparison.CurrentCultureIgnoreCase);
        }

        #endregion

        #region Free

        public static void Free<T>(this T self, ref T obj) where T : class
        {
            Free(ref obj);
        }

        public static void Free(this object self)
        {
            Free(ref self);
        }

        public static void Free<T>(ref T obj) where T : class
        {
            if (obj == null) return;
            T obj1 = obj;
            obj = null;
            if (obj1 is IDisposable)
                (obj1 as IDisposable).Dispose();
        }

        public static void FreeEx<T>(ref T obj) where T : class
        {
            if (obj == null) return;
            T _obj = obj;
            GC.SuppressFinalize(obj);
            GC.ReRegisterForFinalize(obj);
            obj = null;
            if (_obj is IDisposable)
                (_obj as IDisposable).Dispose();
            GC.SuppressFinalize(_obj);
            GC.ReRegisterForFinalize(_obj);
            _obj = null;
        }

        #endregion

        #region Exchange

        public static void Exchange<T>(ref T var1, ref T var2)
        {
            T temp = var1;
            var1 = var2;
            var2 = temp;
        }

        #endregion

        #region Serialize/Deserialize

        public static string SerializeToXml<TClass>(this TClass self) where TClass : class
        {
            if (self == null)
                return null;
            var s = new XmlSerializer(typeof (TClass));
            using (var sw = new StringWriter())
            {
                s.Serialize(sw, self);
                return sw.ToString();
            }
        }

        public static byte[] SerializeToXmlByteArray<TClass>(this TClass self) where TClass : class
        {
            if (self == null)
                return null;
            return Encoding.Unicode.GetBytes(self.SerializeToXml());
        }

        public static TClass DeserializeFromXml<TClass>(this string xml) where TClass : class
        {
            if (xml.IsEmpty(true)) return null;
            var s = new XmlSerializer(typeof (TClass));
            using (var sr = new StringReader(xml))
            {
                return s.Deserialize(sr).As<TClass>();
            }
        }

        public static TClass DeserializeFromXml<TClass>(this byte[] xml) where TClass : class
        {
            if (xml == null) return null;
            return Encoding.Unicode.GetString(xml).DeserializeFromXml<TClass>();
        }

        public static bool EliminationCycles(this object obj, List<object> exclude)
        {
            if (obj == null) return false;

            Type type = obj.GetType();
            if (type.IsPrimitive || Reflection.IsTypeNullableByName(type.FullName) || type == typeof (Guid) ||
                type == typeof (String) || type == typeof (DateTime) || type.IsEnum)
            {
                return false;
            }

            if (exclude == null || exclude.Count == 0) return false;

            if (obj is IEnumerable)
            {
                var remList = new List<object>();
                var lst = (obj as IEnumerable);
                foreach (object item in lst)
                {
                    if (EliminationCycles(item, exclude)) remList.Add(item);
                    //if (!(obj as IEnumerable).GetEnumerator().MoveNext()) break;
                }
                foreach (object item in remList)
                    obj.GetType().InvokeMember("Remove", BindingFlags.InvokeMethod, null, obj, new[] {item});
                return false;
            }

            object fnd = exclude.Where(a => ReferenceEquals(a, obj)).FirstOrDefault();
            if (fnd != null) return true;

            //locExclude.Add(obj);

            IEnumerable<PropertyInfo> pList =
                obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public).Where(
                    a => a.GetCustomAttributes(typeof (DataMemberAttribute), false).Length > 0);
            foreach (PropertyInfo pr in pList)
            {
                if (EliminationCycles(obj.GetPropValue(pr.Name), exclude))
                {
                    obj.SetPropValue(pr.Name, Activator.CreateInstance(pr.PropertyType));
                    obj.SetPropValue(pr.Name, null);
                    continue;
                }
            }
            return false;
        }

        public static void EliminationCycles(this IEnumerable list, List<object> exclude)
        {
            foreach (object obj in list)
                EliminationCycles(obj, exclude);
        }

        #endregion

        #region AsString

        /// <summary>
        /// Converts object to string. Returns default value for null objects.
        /// (for old framework versions)
        /// </summary>
        /// <param name="obj">Converting object</param>
        /// <returns>String representation of object</returns>
        public static string AsString(this object obj)
        {
            return obj.AsString("");
        }

        /// <summary>
        /// Converts object to string. Returns default value for null objects.
        /// </summary>
        /// <param name="obj">Converting object</param>
        /// <param name="defValue">Value that will be return if object is null</param>
        /// <returns>String representation of object</returns>
        public static string AsString(this object obj, string defValue = "")
        {
            if (obj.IsNull()) return defValue;
            return obj.ToString();
        }

        public static string AsString(this Int16? obj, string format)
        {
            return obj.AsString(format, "");
        }

        public static string AsString(this Int16? obj, string format, string defValue)
        {
            return !obj.IsNull() ? obj.Value.ToString(format) : defValue;
        }

        public static string AsString(this int? obj, string format)
        {
            return obj.AsString(format, "");
        }

        public static string AsString(this int? obj, string format, string defValue)
        {
            return !obj.IsNull() ? obj.Value.ToString(format) : defValue;
        }

        public static string AsString(this double? obj, string format)
        {
            return obj.AsString(format, "");
        }

        public static string AsString(this double? obj, string format, string defValue)
        {
            return !obj.IsNull() ? obj.Value.ToString(format) : defValue;
        }

        public static string AsString(this float? obj, string format)
        {
            return obj.AsString(format, "");
        }

        public static string AsString(this float? obj, string format, string defValue)
        {
            return !obj.IsNull() ? obj.Value.ToString(format) : defValue;
        }

        #endregion

        #region AsInt

        /// <summary>
        /// Converts object to integer. Returns default value for null objects.
        /// </summary>
        /// <param name="obj">Converting object</param>
        /// <param name="defValue">Value that will be return if object is null</param>
        /// <returns>Integer representation of object</returns>
        public static int AsInt(this object obj, int defValue = 0)
        {
            int res = defValue;
            try
            {
                if (obj.IsNull()) return defValue;
                if (obj is string)
                {
                    if (!Int32.TryParse(obj.AsString(), out res))
                        res = defValue;
                }
                else if (obj.GetType().IsNumber() || obj.GetType().IsEnum)
                    res = Convert.ToInt32(obj);
                else res = (int) obj;
            }
            catch
            {
            }
            return res;
        }

        #endregion
    }
}