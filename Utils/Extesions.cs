
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Utils
{

    public static class ObjectExtension
    {
        public static bool IsNull(this object obj)
        {
            return (obj == null || obj == DBNull.Value);
        }
        public static bool IsNotNull(this object obj)
        {
            return (obj != null && obj != DBNull.Value);
        }

        #region AsString

        public static string AsString(this object obj)
        {
            return obj.AsString("");
        }

        public static string AsString(this object obj, string defValue)
        {
            return !obj.IsNull() ? obj.ToString() : defValue;
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

        public static int AsInt(this object obj)
        {
            return obj.AsInt(0);
        }

        public static int AsInt(this object obj, int defValue)
        {
            var res = defValue;
            try
            {
                res = !obj.IsNull() ? Convert.ToInt32(obj) : defValue;
            }
            catch { }
            return res;
        }

        #endregion

        #region AsFloat = AsDouble

        public static double AsFloat(this object obj)
        {
            return obj.AsDouble(0);
        }

        public static double AsFloat(this object obj, double defValue)
        {
            return obj.AsDouble(defValue);
        }

        public static double AsDouble(this object obj)
        {
            return obj.AsDouble(0);
        }

        public static double AsDouble(this object obj, double defValue)
        {
            var res = defValue;
            try
            {
                if (!obj.IsNull() && Double.TryParse(obj.ToString(), out defValue))
                    res = defValue;
            }
            catch { }
            return res;
        }

        #endregion

        #region AsDecimal

        public static decimal AsDecimal(this object obj)
        {
            return obj.AsDecimal(0);
        }

        public static decimal AsDecimal(this object obj, decimal defValue)
        {
            var res = defValue;
            try
            {
                if (!obj.IsNull() && Decimal.TryParse(obj.ToString(), out defValue))
                    res = defValue;
            }
            catch { }
            return res;
        }

        #endregion

        #region AsBool

        public static bool AsBool(this object obj)
        {
            return obj.AsBool(false);
        }

        public static bool AsBool(this object obj, bool defValue)
        {
            var res = defValue;
            try
            {
                if (!obj.IsNull())
                    if (obj.AsString() == "0" || obj.AsString().SameText("false") || obj.AsString() == "")
                        res = false;
                    else
                        res = true;
            }
            catch { }
            return res;
        }

        #endregion

        #region AsDateTime

        public static DateTime AsDateTime(this object obj)
        {
            return obj.AsDateTime(DateTime.MinValue);
        }

        public static DateTime AsDateTime(this object obj, DateTime defValue)
        {
            var res = defValue;
            try
            {
                if (!obj.IsNull() && DateTime.TryParse(obj.ToString(), out defValue))
                    res = defValue;
            }
            catch { }
            return DateTime.SpecifyKind(res, DateTimeKind.Unspecified);
        }

        #endregion

        #region AsDateTimeWithoutMs

        public static DateTime AsDateTimeWithoutMs(this object obj)
        {
            return obj.AsDateTimeWithoutMs(DateTime.MinValue);
        }

        public static DateTime AsDateTimeWithoutMs(this object obj, DateTime defValue)
        {
            var res = defValue;
            try
            {
                if (!obj.IsNull() && DateTime.TryParse(obj.ToString(), out defValue))
                    res = defValue;
            }
            catch { }
            return DateTime.SpecifyKind(res.AddMilliseconds(-res.Millisecond), DateTimeKind.Unspecified);
        }

        #endregion

        #region AsDate

        /// <summary>
        /// Если время меньше 12-00, то возвращаем указанную дату без времени.
        /// Если после или равно 12-00, то возвращаем указанную дату без времени + 1 день.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static DateTime RoundDate(this DateTime dt)
        {
            DateTime resDate;
            if (dt.Hour < 12)
                resDate = dt.Date;
            else
                resDate = dt.Date.AddDays(1);

            resDate = DateTime.SpecifyKind(resDate, DateTimeKind.Unspecified);

            return resDate;
        }

        public static DateTime AsDate(this object obj)
        {
            return obj.AsDate(DateTime.MinValue);
        }

        public static DateTime AsDate(this object obj, DateTime DefValue)
        {
            var res = DefValue.Date;
            try
            {
                if (!obj.IsNull() && DateTime.TryParse(obj.ToString(), out DefValue))
                    res = DefValue.Date;
            }
            catch { }
            return DateTime.SpecifyKind(res, DateTimeKind.Unspecified);
        }

        public static DateTime? AsDateOrNull(this object obj)
        {
            return obj.IsNotNull() ? obj.AsDate() : (DateTime?)null;
        }

        #endregion

        #region AsTime

        public static TimeSpan AsTime(this object obj)
        {
            return obj.AsTime(DateTime.MinValue);
        }

        public static TimeSpan AsTime(this object obj, DateTime DefValue)
        {
            var res = DefValue.TimeOfDay;
            try
            {
                if (!obj.IsNull() && DateTime.TryParse(obj.ToString(), out DefValue))
                    res = DefValue.TimeOfDay;
            }
            catch { }
            return res;
        }

        #endregion

        #region AsSQL

        public static string AsSQL(this object obj)
        {
            return AsSQL(obj, "Null");
        }

        public static string AsSQL(this object obj, string DefValue)
        {
            if (obj.IsNull())
                return DefValue;
            if (obj.GetType() == typeof(string))
                return obj.ToString().QuotedStr();
            else if (obj.GetType() == typeof(DateTime))
                return obj.AsDate().ToString("dd.MM.yyyy").QuotedStr();
            else if (obj.GetType() == typeof(bool))
                return obj.AsBool() ? "1" : "0";
            else
                return obj.AsString().Replace(',', '.');
        }

        #endregion

        #region As

        public static T As<T>(this object obj)
        {
            return obj.As(default(T));
        }
        public static T As<T>(this object obj, T defValue)
        {
            var res = defValue;
            try
            {
                if (!obj.IsNull())
                {
                    if (typeof(T) == typeof(string)) res = (T)(object)obj.AsString();
                    else if (typeof(T) == typeof(int) || typeof(T) == typeof(int?)) res = (T)(object)obj.AsInt();
                    else if (typeof(T) == typeof(double) || typeof(T) == typeof(double?)) res = (T)(object)obj.AsFloat();
                    else if (typeof(T) == typeof(decimal) || typeof(T) == typeof(decimal?)) res = (T)(object)obj.AsDecimal();
                    else if (typeof(T) == typeof(bool) || typeof(T) == typeof(bool?)) res = (T)(object)obj.AsBool();
                    else if (typeof(T) == typeof(DateTime) || typeof(T) == typeof(DateTime?)) res = (T)(object)obj.AsDateTime();
                    else res = (T)obj;
                }
            }
            catch { }
            return res;
        }

        #endregion

        #region IsPropsEquals

        /// <summary>
        /// <para>Сравнение двух объектов, путем сравнения значений каждого свойства объета.</para>
        /// <para>Проверяются только public свойства.</para>
        /// </summary>
        public static bool IsPropsEquals(this object self, object obj, bool ignoreNull = false)
        {
            if (self == null && obj == null) return true;
            if (self == null || obj == null) return false;
            var res = self.Equals(obj);
            if (self.IsPrimitiveType() || obj.IsPrimitiveType()) return res;
            if (!res)
            {
                res = true;
                Type T1 = self.GetType();
                Type T2 = obj.GetType();
                var PL1 = T1.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
                var PL2 = T2.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
                object V1;
                object V2;
                foreach (var P1 in PL1)
                {
                    V1 = P1.GetValue(self, null);
                    if (ignoreNull && V1 == null) continue;
                    if (T1 == T2)
                        V2 = P1.GetValue(obj, null);
                    else
                    {
                        V2 = V1;
                        foreach (var P2 in PL2)
                        {
                            if (P2.Name == P1.Name)
                            {
                                V2 = P2.GetValue(obj, null);
                                break;
                            }
                        }
                    }
                    res = V1.IsPropsEquals(V2);
                    if (!res) break;
                }
            }
            return res;
        }

        #endregion

        #region IsFieldsEquals

        /// <summary>
        /// <para>Сравнение двух объектов, путем сравнения значений каждого поля объета.</para>
        /// <para>Проверяются только public поля.</para>
        /// </summary>
        public static bool IsFieldsEquals(this object self, object obj, bool ignoreNull = false)
        {
            if (self == null && obj == null) return true;
            if (self == null || obj == null) return false;
            var res = self.Equals(obj);
            if (self.IsPrimitiveType() || obj.IsPrimitiveType()) return res;
            if (!res)
            {
                res = true;
                Type T1 = self.GetType();
                Type T2 = obj.GetType();
                var PL1 = T1.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
                var PL2 = T2.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
                object V1;
                object V2;
                foreach (var P1 in PL1)
                {
                    V1 = P1.GetValue(self);
                    if (ignoreNull && V1 == null) continue;
                    if (T1 == T2)
                        V2 = P1.GetValue(obj);
                    else
                    {
                        V2 = V1;
                        foreach (var P2 in PL2)
                        {
                            if (P2.Name == P1.Name)
                            {
                                V2 = P2.GetValue(obj);
                                break;
                            }
                        }
                    }
                    res = V1.IsFieldsEquals(V2);
                    if (!res) break;
                }
            }
            return res;
        }

        #endregion

        #region IsEquals

        /// <summary>
        /// <para>Сравнение двух объектов, путем сравнения значений каждого поля и свойства объета.</para>
        /// <para>Проверяются только public поля и свойства.</para>
        /// </summary>
        public static bool IsEquals(this object self, object obj, bool ignoreNull = false)
        {
            return (self.IsFieldsEquals(obj, ignoreNull) && self.IsPropsEquals(obj, ignoreNull));
        }

        #endregion

        #region ExtFields methods

        /// <summary>
        /// <para>Получить ссылку на public поле объекта по имени.</para>
        /// <para>Если такого поля нет - возвращает null.</para>
        /// </summary>
        private static FieldInfo FieldByName(this object self, string fieldName)
        {
            FieldInfo res = null;
            if (self != null && fieldName != "")
            {
                var T = self.GetType();
                var pl = T.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
                res = pl.FirstOrDefault(p => p.Name == fieldName);
            }
            return res;
        }

        /// <summary>
        /// <para>Получить значение public поля объекта по имени.</para>
        /// <para>Если такого поля нет - возвращает null.</para>
        /// </summary>
        public static object GetFieldValue(this object self, string fieldName)
        {
            var pi = self.FieldByName(fieldName);
            return pi != null ? pi.GetValue(self) : null;
        }

        /// <summary>
        /// <para>Установить значение public поля объекта по имени.</para>
        /// <para>Если такого поля нет - ничего не делает.</para>
        /// <para>Возвращает предыдущее значение свойства.</para>
        /// </summary>
        public static object SetFieldValue(this object self, string fieldName, object fieldValue)
        {
            var pi = self.FieldByName(fieldName);
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
        /// <para>Копирование значений простых полей с одинаковыми именами в другой объект.</para>
        /// <para>Копируются только public поля.</para>
        /// </summary>
        public static T CopyFieldsTo<T>(this object self, T obj, bool ignoreNulls = false) where T : class
        {
            if (self == null || obj == null) return obj;
            var T1 = self.GetType();
            var T2 = obj.GetType();
            var PL1 = T1.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
            var PL2 = T2.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
            foreach (var P1 in PL1)
            {
                if (P1.FieldType.IsPrimitive || P1.FieldType == typeof(String) || P1.FieldType == typeof(DateTime))
                {
                    var V = P1.GetValue(self);
                    if (ignoreNulls && V.IsNull()) continue;
                    if (T1 == T2)
                        P1.SetValue(obj, V);
                    else
                        foreach (var P2 in PL2)
                        {
                            if (P2.Name == P1.Name)
                            {
                                P2.SetValue(obj, V);
                                break;
                            }
                        }
                }
            }
            return obj;
        }
        /// <summary>
        /// <para>Копирование значений простых полей с одинаковыми именами из другого объекта.</para>
        /// <para>Копируются только public поля.</para>
        /// </summary>
        public static T CopyFieldsFrom<T>(this T self, object obj) where T : class
        {
            return obj.CopyFieldsTo(self);
        }

        #endregion

        #region ExtProps methods
        
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
                res = (T)self.GetType().InvokeMember(methodName, BindingFlags.InvokeMethod, null, self, args);
            }
            return res;
        }

        /// <summary>
        /// <para>Получить ссылку на public свойство объекта по имени.</para>
        /// <para>Если такого свойства нет - возвращает null.</para>
        /// </summary>
        internal static PropertyInfo PropByName(this object self, string propName)
        {
            PropertyInfo res = null;
            if (self != null && propName != "")
            {
                var T = self.GetType();
                var pl = T.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
                res = pl.FirstOrDefault(p => p.Name == propName);
            }
            return res;
        }

        /// <summary>
        /// <para>Получить значение public свойства объекта по имени.</para>
        /// <para>Если такого свойства нет - возвращает null.</para>
        /// </summary>
        /// <author>Lion</author>
        public static object GetPropValue(this object self, string propName)
        {
            var pi = self.PropByName(propName);
            return pi != null ? pi.GetValue(self, null) : null;
        }

        /// <summary>
        /// <para>Установить значение public свойства объекта по имени.</para>
        /// <para>Если такого свойства нет - ничего не делает.</para>
        /// <para>Возвращает предыдущее значение свойства.</para>
        /// </summary>
        public static object SetPropValue(this object self, string propName, object propValue)
        {
            var pi = self.PropByName(propName);
            object res = null;
            if (pi != null)
            {
                res = pi.GetValue(self, null);
                pi.SetValue(self, propValue, null);
            }
            return res;
        }

        #endregion

        #region CopyProps

        /// <summary>
        /// <para>Копирование значений простых свойств с одинаковыми именами в другой объект.</para>
        /// <para>Копируются только public свойства.</para>
        /// <para>Параметр recurse = true позволяет автоматически создавать и копировать вложенные объектные свойства.</para>
        /// </summary>
        public static T CopyPropsTo<T>(this object self, T obj, bool ignoreNulls = false, bool recurse = false) where T : class
        {
            if (self == null || obj == null) return obj;
            var t1 = self.GetType();
            var t2 = obj.GetType();
            var pl1 = t1.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
            var pl2 = t2.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
            foreach (var p1 in pl1)
            {
                if (((!p1.PropertyType.IsPrimitive && p1.PropertyType != typeof(String)) &&
                     p1.PropertyType != typeof(DateTime)) && !p1.PropertyType.FullName.Contains("System.Nullable`1[["))
                {
                    // Это не простой тип, который можно просто скопировать. 
                    // то можно попробовать скопировать его, если recurse = true          
                    /*
                              if (p1.PropertyType.IsArray)
                              {
                                // Если это массив 
                                var sarr = p1.GetValue(self, null);
                                var pi2 = obj.PropByName(p1.Name);
                                if (pi2.IsNotNull())
                                  pi2.SetValue(obj, sarr.CopyArrayOrList(), null);
                              }
                              else if (p1.PropertyType.IsClass)
                              {
                                // Если это объект
            
                              }
                    */
                    continue;
                }
                var v = p1.GetValue(self, null);
                if (ignoreNulls && v.IsNull()) continue;
                foreach (var p2 in pl2)
                {
                    if (t1 == t2)
                        p1.SetValue(obj, v, null);
                    else
                        if (p2.Name == p1.Name)
                        {
                            if (p2.GetValue(obj, null) != v && p2.GetSetMethod(true) != null)
                                p2.SetValue(obj, v, null);
                            break;
                        }
                }
            }
            return obj;
        }
        /*
            public static object CopyArrayOrList(this object source)
            {
              object res = null;
              if (source != null)
              {
                if (source.GetType().IsArray)
                {
                  res = Activator.CreateInstance(source.GetType());
                  var i = 0;
                  foreach (var item in (Array)source)
                  {
                    ((Array) res)[i++] = item;
                  }
                }
                else if (source is IList)
                {
                  foreach (var item in (IList)source)
                  {

                  }
                }
              }
              return res;
            }
        */

        public static T CopyPropsTo<T>(this object self, T obj, bool ignoreNulls = false) where T : class
        {
            return CopyPropsTo(self, obj, ignoreNulls, false);
        }

        /// <summary>
        /// <para>Копирование значений простых свойств с одинаковыми именами из другого объекта.</para>
        /// <para>Копируются только public свойства.</para>
        /// </summary>
        public static T CopyPropsFrom<T>(this T self, object obj, bool ignoreNulls = false) where T : class
        {
            return obj.CopyPropsTo(self, ignoreNulls);
        }

        #endregion

        #region Copy

        /// <summary>
        /// <para>Копирование значений простых полей и свойств с одинаковыми именами в другой объект.</para>
        /// <para>Копируются только public поля и свойства.</para>
        /// </summary>
        public static T CopyTo<T>(this object self, T obj, bool ignoreNulls = false) where T : class
        {
            self.CopyFieldsTo(obj, ignoreNulls);
            self.CopyPropsTo(obj, ignoreNulls);
            return obj;
        }
        public static T CopyTo<T>(this object self, bool ignoreNulls = false) where T : class, new()
        {
            var obj = new T();
            self.CopyFieldsTo(obj, ignoreNulls);
            self.CopyPropsTo(obj, ignoreNulls);
            return obj;
        }

        /// <summary>
        /// <para>Копирование значений простых свойств с одинаковыми именами из другого объекта.</para>
        /// <para>Копируются только к public поля.</para>
        /// </summary>
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
        public static bool SameText(this object self, object obj)
        {
            return String.Compare(self.AsString(), obj.AsString(), true) == 0;
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
            var _obj = obj;
            obj = null;
            if (_obj is IDisposable)
                (_obj as IDisposable).Dispose();
        }


        public static void FreeEx(this object self)
        {
            FreeEx(ref self);
        }

        public static void FreeEx<T>(ref T obj) where T : class
        {
            if (obj == null) return;
            var _obj = obj;
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

    }

    public static class HashtableExtension
    {
        #region GetValue

        /// <summary>
        /// <para>Получение значения из хеш-таблицы.</para>
        /// <para>Eсли такого значения в таблице нет - возвращается значение по умолчанию.</para>
        /// </summary>
        public static object GetValue(this Hashtable self, object key, object defValue)
        {
            if (!self.IsNull() && self.ContainsKey(key) && self[key] != null)
                return self[key];
            else
                return defValue;
        }
        public static object GetValue(this Hashtable self, object key)
        {
            return GetValue(self, key, null);
        }
        public static object GetValue(this Dictionary<string, object> self, string key, object defValue)
        {
            if (!self.IsNull() && self.ContainsKey(key) && self[key] != null)
                return self[key];
            else
                return defValue;
        }
        public static object GetValue(this Dictionary<string, object> self, string key)
        {
            return GetValue(self, key, null);
        }

        #endregion

        #region GetString

        public static string GetString(this Hashtable self, object key)
        {
            return GetString(self, key, "");
        }
        public static string GetString(this Hashtable self, object key, string defValue)
        {
            if (!self.IsNull() && self.ContainsKey(key) && self[key] != null)
                return self[key].AsString(defValue);
            else
                return defValue;
        }

        public static string GetString(this Dictionary<string, object> self, string key)
        {
            return GetString(self, key, "");
        }
        public static string GetString(this Dictionary<string, object> self, string key, string defValue)
        {
            if (!self.IsNull() && self.ContainsKey(key) && self[key] != null)
                return self[key].AsString(defValue);
            else
                return defValue;
        }

        #endregion

        #region GetInt

        public static int GetInt(this Hashtable self, object key)
        {
            return GetInt(self, key, 0);
        }
        public static int GetInt(this Hashtable self, object key, int DefValue)
        {
            if (!self.IsNull() && self.ContainsKey(key) && self[key] != null)
                return self[key].AsInt(DefValue);
            else
                return DefValue;
        }

        public static int GetInt(this Dictionary<string, object> self, string key)
        {
            return GetInt(self, key, 0);
        }
        public static int GetInt(this Dictionary<string, object> self, string key, int defValue)
        {
            if (!self.IsNull() && self.ContainsKey(key) && self[key] != null)
                return self[key].AsInt(defValue);
            else
                return defValue;
        }

        #endregion

        #region GetFloat

        public static double GetFloat(this Hashtable self, object key)
        {
            return GetFloat(self, key, 0);
        }
        public static double GetFloat(this Hashtable self, object key, double defValue)
        {
            if (!self.IsNull() && self.ContainsKey(key) && self[key] != null)
                return self[key].AsFloat(defValue);
            else
                return defValue;
        }

        public static double GetFloat(this Dictionary<string, object> self, string key)
        {
            return GetFloat(self, key, 0);
        }
        public static double GetFloat(this Dictionary<string, object> self, string key, double defValue)
        {
            if (!self.IsNull() && self.ContainsKey(key) && self[key] != null)
                return self[key].AsFloat(defValue);
            else
                return defValue;
        }

        #endregion

        #region GetBool

        public static bool GetBool(this Hashtable self, object key)
        {
            return GetBool(self, key, false);
        }
        public static bool GetBool(this Hashtable self, object key, bool defValue)
        {
            if (!self.IsNull() && self.ContainsKey(key) && self[key] != null)
                return self[key].AsBool(defValue);
            else
                return defValue;
        }

        public static bool GetBool(this Dictionary<string, object> self, string key)
        {
            return GetBool(self, key, false);
        }
        public static bool GetBool(this Dictionary<string, object> self, string key, bool defValue)
        {
            if (!self.IsNull() && self.ContainsKey(key) && self[key] != null)
                return self[key].AsBool(defValue);
            else
                return defValue;
        }
        #endregion

        #region GetDateTime

        public static DateTime GetDateTime(this Hashtable self, object key)
        {
            return GetDateTime(self, key, DateTime.MinValue);
        }
        public static DateTime GetDateTime(this Hashtable self, object key, DateTime defValue)
        {
            if (!self.IsNull() && self.ContainsKey(key) && self[key] != null)
                return self[key].AsDateTime(defValue);
            else
                return defValue;
        }

        public static DateTime GetDateTime(this Dictionary<string, object> self, string key)
        {
            return GetDateTime(self, key, DateTime.MinValue);
        }
        public static DateTime GetDateTime(this Dictionary<string, object> self, string key, DateTime defValue)
        {
            if (!self.IsNull() && self.ContainsKey(key) && self[key] != null)
                return self[key].AsDateTime(defValue);
            else
                return defValue;
        }
        #endregion

        #region Get<T>

        /// <summary>
        /// <para>Получение значения из хеш-таблицы.</para>
        /// <para>Eсли такого значения в таблице нет - возвращается значение по умолчанию.</para>
        /// </summary>
        public static T Get<T>(this Hashtable self, object key, T defValue)
        {
            if (!self.IsNull() && self.ContainsKey(key) && self[key] != null)
                return self[key].As(defValue);
            else
                return defValue;
        }
        public static T Get<T>(this Hashtable self, object key)
        {
            return Get(self, key, default(T));
        }
        public static T Get<T>(this Dictionary<string, object> self, string key, T defValue)
        {
            if (!self.IsNull() && self.ContainsKey(key) && self[key] != null)
                return self[key].As(defValue);
            else
                return defValue;
        }
        public static T Get<T>(this Dictionary<string, object> self, string key)
        {
            return Get(self, key, default(T));
        }
        #endregion

        public static bool IsNull(this Dictionary<string, object> self, string key)
        {
            return self == null || !self.ContainsKey(key) || self[key].IsNull();
        }
        public static bool IsNotNull(this Dictionary<string, object> self, string key)
        {
            return !IsNull(self, key);
        }

        public static string AsString(this Hashtable self)
        {
            return Strings.GetParametersHashtableAsString(self);
        }

    }

    public static class StringExtension
    {
        #region LeftPart

        /// <summary>
        /// Возвращает левую часть строки
        /// </summary>
        /// <param name="S"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string LeftPart(this string S, int length)
        {
            if (length < 0)
                return "";
            else if (length >= S.Length)
                return S;
            else
                return S.Substring(0, length);
        }

        #endregion

        #region RightPart

        /// <summary>
        /// Возвращает правую часть строки
        /// </summary>
        /// <param name="S"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string RightPart(this string S, int length)
        {
            if (length < 0)
                return "";
            else if (length >= S.Length)
                return S;
            else
                return S.Substring(S.Length - length, length);
        }

        #endregion

        #region QuotedStr

        /// <summary>
        /// Возвращает строку в указанных кавычках с правильным удвоением символа кавычки внутри строки
        /// </summary>
        /// <param name="S"></param>
        /// <param name="quote"></param>
        /// <returns></returns>
        public static string QuotedStr(this string S, char quote) { return Strings.QuotedStr(S, quote); }
        /// <summary>
        /// Возвращает строку в одинарных кавычках с правильным удвоением символа кавычки внутри строки
        /// </summary>
        /// <param name="S"></param>
        /// <returns></returns>
        public static string QuotedStr(this string S) { return Strings.QuotedStr(S); }

        #endregion

        #region DequotedStr

        /// <summary>
        /// <para>Извлекает строку из кавычек, но только в том случае, если вся строка заключена в кавычки.</para>
        /// <para>Если строка не полностью заключена в кавычки или неправильно "закавычена" - просто возвращает исходную строку.</para>
        /// <para>Кавычками считаются символы одинарной и двойной кавычки.</para>
        /// </summary>
        /// <param name="S"></param>
        /// <returns></returns>
        public static string DequotedStr(this string S) { return Strings.DequotedStr(S); }
        /// <summary>
        /// <para>Извлекает строку из кавычек, но только в том случае, если вся строка заключена в кавычки.</para>
        /// <para>Если строка не полностью заключена в кавычки или неправильно "закавычена" - просто возвращает исходную строку.</para>
        /// <para>Кавычками считаются либой из символов строки, переданной в параметре quotes.</para>
        /// </summary>
        /// <param name="S"></param>
        /// <param name="quotes"></param>
        /// <returns></returns>
        public static string DequotedStr(this string S, string quotes) { return Strings.DequotedStr(S, quotes); }

        #endregion
        
        #region ReplaceText

        /// <summary>
        /// Заменяет подстроку в строке без учета регистра (используются регекспы)
        /// </summary>
        public static string ReplaceText(this string S, string oldValue, string newValue)
        {
            return Regex.Replace(S, oldValue, newValue, RegexOptions.IgnoreCase);
        }

        #endregion

        public static string MD5(this string s)
        {
            MD5CryptoServiceProvider provider;
            provider = new MD5CryptoServiceProvider();
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            var builder = new StringBuilder();

            bytes = provider.ComputeHash(bytes);

            foreach (byte b in bytes)
                builder.Append(b.ToString("x2").ToLower());

            return builder.ToString();
        }

        public static bool IsEmpty(this string str)
        {
            return (String.IsNullOrEmpty(str));
        }
        public static bool IsNotEmpty(this string str)
        {
            return !str.IsEmpty();
        }

        public static string AddText(this string str, string addStr, string separator)
        {
            if (str == "") return addStr;
            return str + separator + addStr;
        }

        public static string Fmt(this string str, params object[] args)
        {
            return String.Format(str, args);
        }

        public static string FmtQuery(this string str, Hashtable args)
        {
            return Strings.FmtQuery(str, args);
        }

        #region Convert

        public static string Convert(this string str, string sourceEncoding, string destEncoding)
        {
            return Convert(str, Encoding.GetEncoding(sourceEncoding), Encoding.GetEncoding(destEncoding));
        }
        public static string Convert(this string str, Encoding sourceEncoding, Encoding destEncoding)
        {
            return str.IsEmpty() ? str : destEncoding.GetString(sourceEncoding.GetBytes(str));
        }

        #endregion

        #region To

        public static string ToLowCaseFirstChar(this string str)
        {
            return str.IsEmpty() ? str : str.Substring(0, 1).ToLower() + str.Substring(1);
        }

        public static string ToUpCaseFirstChar(this string str)
        {
            return str.IsEmpty() ? str : str.Substring(0, 1).ToUpper() + str.Substring(1).ToLower();
        }

        #endregion
    }

    public static class ToHTMLExtension
    {

        public static string Nl2Br(this string s)
        {
            return s.Replace("\r\n", "<br />").Replace("\n", "<br />");
        }

        public static string Tab2Html(this string s, int charCount = 4)
        {
            var tab = "";
            for (var i = 0; i < charCount; i++) tab += "&ensp;"; // Полукегельная шпация (пробел шириной в половину кегля шрифта)
            return s.Replace("\t", tab);
        }

    }

    public static class CollectionBaseExtension
    {
        #region IsCollectionEquals

        /// <summary>
        /// Сравнение значений двух коллекций, путем сравнения каждого элемента
        /// </summary>
        public static bool IsCollectionEqualsByProps(this CollectionBase self, CollectionBase obj)
        {
            bool res = self.Count == obj.Count;
            IList L1 = self as IList;
            IList L2 = obj as IList;
            for (int i = 0; res && i < self.Count; i++)
                res = L1[i].IsPropsEquals(L2[i]);
            return res;
        }

        public static bool IsCollectionEqualsByFields(this CollectionBase self, CollectionBase obj)
        {
            bool res = self.Count == obj.Count;
            IList L1 = self as IList;
            IList L2 = obj as IList;
            for (int i = 0; res && i < self.Count; i++)
                res = L1[i].IsFieldsEquals(L2[i]);
            return res;
        }

        public static bool IsCollectionEquals(this CollectionBase self, CollectionBase obj)
        {
            bool res = self.Count == obj.Count;
            IList L1 = self as IList;
            IList L2 = obj as IList;
            for (int i = 0; res && i < self.Count; i++)
                res = L1[i].IsEquals(L2[i]);
            return res;
        }

        #endregion
    }


    public static class ListOrArrayExtension
    {
        #region Delete

        /// <summary>
        /// Удаление элементов списка, удовлетворяющих условию
        /// </summary>
        public static List<T> Delete<T>(this List<T> self, Func<T, bool> condition)
        {
            for (var i = self.Count - 1; i >= 0; i--)
                if (condition(self[i]))
                    self.RemoveAt(i);
            return self;
        }

        /// <summary>
        /// Удаление элементов массива, удовлетворяющих условию - возвращает список
        /// </summary>
        public static List<T> Delete<T>(this T[] self, Func<T, bool> condition)
        {
            var res = new List<T>();
            for (var i = self.Length - 1; i >= 0; i--)
                if (!condition(self[i]))
                    res.Add(self[i]);
            return res;
        }

        #endregion

        #region ForEach

        /// <summary>
        /// Перечисление элементов массива
        /// </summary>
        public static T[] ForEach<T>(this T[] self, Action<T> action)
        {
            Array.ForEach(self, action);
            return self;
        }

        /// <summary>
        /// Перечисление элементов IEnumerable
        /// </summary>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> self, Action<T> action)
        {
            foreach (var item in self) action(item);
            return self;
        }

        public static IEnumerable ForEach<T>(this IEnumerable self, Action<T> action)
        {
            foreach (T item in self) action(item);
            return self;
        }

        #endregion

        #region Contains

        public static bool Contains<T>(this T[] self, T value)
        {
            return Array.Exists(self, e => e.Equals(value));
        }

        #endregion

        public static string ToStringWithSeparator(this IEnumerable self, string paramSeparator)
        {
            var res = "";
            foreach (var el in self)
                res += el.ToString() + paramSeparator;
            return (res != "") ? res.Substring(0, res.Length - paramSeparator.Length) : res;
        }
    }

    public static class ExceptionExtension
    {

        public static string GetMessageBySource(this Exception Exc, string SourceFilter)
        {
            return ExceptionHandling.Instance.GetExceptionMessageBySource(Exc, SourceFilter);
        }

        public static string GetInnerMessage(this Exception Exc)
        {
            return ExceptionHandling.Instance.GetInnerExceptionMessage(Exc);
        }

        public static string GetFullMessage(this Exception Exc)
        {
            return ExceptionHandling.Instance.GetFullExceptionMessage(Exc);
        }

    }

}
