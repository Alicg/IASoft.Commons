using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Utils.Extensions;

namespace Utils
{
    /// <summary>
    /// работа с отражениями (извлечение информации из классов в рантайме)
    /// </summary>
    public static class Reflection
    {
        public static int MaxNestingLevel = 10;

        public static int MaxContentSize = 1024;

        public static List<Type> AllowTraceAttributes;

        public static string GetFieldListAsStringFromObject(object paramObj)
        {
            return GetFieldListAsStringFromObject(paramObj, 0);
        }

        public static string GetFieldListAsStringFromObject(object paramObj, bool paramWithProperties)
        {
            return GetFieldListAsStringFromObject(paramObj, 0, paramWithProperties);
        }

        /// <summary>
        /// получаем простые объекты или список полей сложного объекта как строку
        /// </summary>
        /// <param name="paramObj">объект</param>
        /// <param name="paramNestingLevel">уровень вложенности (определяет количество пробельных префиксов в начале строки для вывода)</param>
        /// <returns></returns>
        public static string GetFieldListAsStringFromObject(object paramObj, int paramNestingLevel)
        {
            return GetFieldListAsStringFromObject(paramObj, paramNestingLevel, false);
        }

        public static string GetFieldListAsStringFromObject(object paramObj, int paramNestingLevel,
                                                            bool paramWithProperties)
        {
            try
            {
                if (paramObj == null) return "null";

                if (paramNestingLevel > MaxNestingLevel)
                    return "(inner overflow: {0} > {1})".Fmt(paramNestingLevel, MaxNestingLevel);

                string result = "";
                var locPrefixSpaces = (new String(' ', paramNestingLevel*4));

                if (paramObj.IsPrimitiveType())
                {
                    result = paramObj.ToString();
                }
                else
                {
                    //result += "\r\n";
                    if (paramObj is IList)
                    {
                        var locList = paramObj as IList;
                        if (locList != null)
                        {
                            for (int i = 0; i < locList.Count; i++)
                            {
                                result += "\r\n" + locPrefixSpaces + "[" + i + "] : " +
                                          GetFieldListAsStringFromObject(locList[i], paramNestingLevel + 1,
                                                                         paramWithProperties);
                            }
                        }
                    }
                    else if (paramObj is IDictionary)
                    {
                        var locDict = paramObj as IDictionary;
                        if (locDict != null)
                        {
                            foreach (object item in locDict.Keys)
                            {
                                result += "\r\n" + locPrefixSpaces + "[" + item + "] : " +
                                          GetFieldListAsStringFromObject(locDict[item], paramNestingLevel + 1,
                                                                         paramWithProperties);
                            }
                        }
                    }
                    else
                    {
                        Type type = paramObj.GetType();
                        if (type.GetCustomAttribute<LoggableAttribute>() == null) return "(unloggable type)";

                        // поля
                        FieldInfo[] fList = type.GetFields();
                        foreach (FieldInfo fi in fList)
                        {
                            object[] attr = fi.GetCustomAttributes(false);
                            if (AllowTraceAttributes == null ||
                                attr.Where(a => !AllowTraceAttributes.Contains(a.GetType())).FirstOrDefault() != null)
                            {
                                result += "\r\n" + locPrefixSpaces + " " + fi.Name + " [" +
                                          GetNameOfType(fi.FieldType, false) + "] = " +
                                          GetFieldListAsStringFromObject(fi.GetValue(paramObj), paramNestingLevel + 1,
                                                                         paramWithProperties);
                            }
                        }
                        // свойства
                        if (paramWithProperties)
                        {
                            PropertyInfo[] pList = paramObj.GetType().GetProperties();
                            foreach (PropertyInfo pi in pList)
                            {
                                object[] attr = pi.GetCustomAttributes(false);
                                if (AllowTraceAttributes == null ||
                                    attr.Where(a => AllowTraceAttributes.Contains(a.GetType())).Count() > 0)
                                {
                                    result += "\r\n" + locPrefixSpaces + " " + pi.Name + " [" +
                                              GetNameOfType(pi.PropertyType) + "] = " +
                                              GetFieldListAsStringFromObject(pi.GetValue(paramObj, null),
                                                                             paramNestingLevel + 1, paramWithProperties);
                                }
                            }
                        }

                        result = result.Length > MaxContentSize
                                     ? "(size overflow: {0} > {1})".Fmt(result.Length, MaxContentSize)
                                     : result;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                return paramObj + " {PARSE ERROR! : " + ex.Message + "}";
            }
        }

        /// <summary>
        /// определяем корректное имя типа
        /// </summary>
        /// <param name="paramType"></param>
        /// <param name="detailInfo"></param>
        /// <returns></returns>
        private static string GetNameOfType(Type paramType)
        {
            return GetNameOfType(paramType, false);
        }

        /// <summary>
        /// определяем корректное имя типа
        /// </summary>
        /// <param name="paramType"></param>
        /// <param name="detailInfo"></param>
        /// <returns></returns>
        public static string GetNameOfType(Type paramType, bool detailInfo)
        {
            return GetNameOfTypeByString(paramType.FullName, detailInfo);
        }

        /// <summary>
        /// определяем корректное имя типа по его наименованию
        /// </summary>
        /// <param name="paramFullTypeName"></param>
        /// <param name="detailInfo"></param>
        /// <returns></returns>
        private static string GetNameOfTypeByString(string paramFullTypeName, bool detailInfo)
        {
            string result = "";
            int locPos;

            // списки
            locPos = paramFullTypeName.IndexOf("System.Collections.Generic.List`1[[");
            if (locPos != -1)
            {
                result = paramFullTypeName.Replace("System.Collections.Generic.List`1[[", "");
                locPos = result.IndexOf(',');
                if (locPos != -1)
                {
                    return "List<" + GetNameOfTypeByString(result.Substring(0, locPos), detailInfo) + ">";
                }
            }
            // null - совместимые типы
            locPos = paramFullTypeName.IndexOf("System.Nullable`1[[");
            if (locPos != -1)
            {
                result = paramFullTypeName.Replace("System.Nullable`1[[", "");
                locPos = result.IndexOf(',');
                if (locPos != -1)
                {
                    result = GetNameOfTypeByString(result.Substring(0, locPos), detailInfo) + "?";
                    int idx = result.IndexOf('+');
                    if (idx >= 0)
                        result = detailInfo ? result.Replace('+', '.') : result.Substring(idx + 1);
                    return result;
                }
            }

            result = paramFullTypeName
                .Replace('+', '.')
                .Replace("System.", "")
                .Replace("Int32", "int")
                .Replace("Double", "double")
                .Replace("String", "string")
                .Replace("Boolean", "bool")
                .Replace("Byte", "byte");

            // убираем namespace-префиксы
            locPos = -1;
            int locPrevPos = -1;
            do
            {
                locPrevPos = locPos;
                locPos = result.IndexOf('.', locPrevPos + 1);
            } while (locPos != -1);
            result = result.Substring(locPrevPos + 1, result.Length - locPrevPos - 1);
            return result;
        }

        public static bool IsTypeNullableByName(string paramTypeFullName)
        {
            return (paramTypeFullName.IndexOf("System.Nullable`1[[") != -1);
        }

        public static bool IsTypeNullable(Type type)
        {
            if (!type.IsValueType) return true; // ref-type
            return Nullable.GetUnderlyingType(type) != null;
        }

        public static bool IsNullable<T>(T obj)
        {
            if (obj == null) return true; // obvious
            Type type = typeof (T);
            if (!type.IsValueType) return true; // ref-type
            return Nullable.GetUnderlyingType(type) != null;
        }

        //public static bool IsPrimitiveType(object obj)
        //{
        //    return (obj.GetType().IsPrimitive || (obj is Guid) || (obj is String) || (obj is DateTime) || (obj.GetType().IsEnum));
        //}

        public static bool IsPrimitive<T>(T obj)
        {
            return IsPrimitiveByType(typeof (T));
        }

        public static bool IsPrimitiveByType(Type type)
        {
            if (type == null) return false;
            return (type.IsPrimitive || (type == typeof (Guid)) || (type == typeof (String)) ||
                    (type == typeof (DateTime)) || (type.IsEnum) || IsPrimitiveByType(Nullable.GetUnderlyingType(type)));
        }
    }

    public static class ReflectionExt
    {
        //public static string ToText(this IDictionary paramDictionary)
        //{
        //    var res = "";
        //    foreach (var obj in paramDictionary.Keys)
        //    {
        //        res += (res == "" ? "" : "\r\n") + obj.ToTextEx() + " : " + paramDictionary[obj].ToTextEx();
        //    }
        //    return res;
        //}

        //public static string ToText(this IList paramList)
        //{
        //    var res = "";
        //    foreach (var obj in paramList)
        //    {
        //        res += (res == "" ? "" : "\r\n") + obj.ToTextEx();
        //    }
        //    return res;
        //}

        //public static string ToText(this IList paramList, bool withProps)
        //{
        //    var res = "";
        //    foreach (var obj in paramList)
        //    {
        //        res += (res == "" ? "" : "\r\n") + obj.ToTextEx(withProps);
        //    }
        //    return res;
        //}

        public static string ToTextEx(this object self)
        {
            //if (self is String) return self.ToString();
            //if (self is IList) return (self as IList).ToText();
            //if (self is IDictionary) return (self as IDictionary).ToText();
            return Reflection.GetFieldListAsStringFromObject(self);
        }

        public static string ToTextEx(this object self, bool withProps)
        {
            //if (self is String) return self.ToString();
            //if (self is IList) return (self as IList).ToText(withProps);
            //if (self is IDictionary) return (self as IDictionary).ToText();
            return Reflection.GetFieldListAsStringFromObject(self, withProps);
        }

        public static string ToSqlCondition<T>(this List<T> self)
        {
            string res = "";
            if (self == null) return res;
            foreach (T val in self)
                res += (res == "" ? "" : ", ") + val.AsInt().AsString();
            return res == "" ? "" : "(" + res + ")";
        }

        public static bool IsTypeNullable(this Type selfType)
        {
            return Reflection.IsTypeNullable(selfType);
        }

        public static bool IsNullable<T>(this T obj)
        {
            return Reflection.IsNullable(obj);
        }

        public static bool IsPrimitiveType(this object obj)
        {
            return Reflection.IsPrimitiveByType(obj.GetType());
        }

        public static bool IsPrimitive<T>(this T obj)
        {
            return Reflection.IsPrimitive(obj);
        }

        public static bool IsPrimitive(this Type type)
        {
            return Reflection.IsPrimitiveByType(type);
        }

        public static T GetCustomAttribute<T>(this Type self, bool inherit = false)
        {
            return (T) self.GetCustomAttributes(inherit).Where(a => a is T).FirstOrDefault();
        }

        public static T GetCustomAttribute<T>(this FieldInfo self, bool inherit = false)
        {
            return (T) self.GetCustomAttributes(inherit).Where(a => a is T).FirstOrDefault();
        }
    }
}