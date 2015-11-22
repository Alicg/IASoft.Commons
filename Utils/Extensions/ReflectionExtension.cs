using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Utils.Extensions
{
    public static class ReflectionExtension
    {
        private static TAttribute InternalGet<TAttribute>(this IEnumerable<object> attrs,
                                                          Func<TAttribute, bool> condition = null)
            where TAttribute : Attribute
        {
            IEnumerable<TAttribute> x = (from TAttribute attr in attrs
                                         let order =
                                             attr.GetType().GetCustomAttributes(typeof (OrderAttribute), true).Cast
                                             <OrderAttribute>().Select(a => a.Position).FirstOrDefault()
                                         where
                                             (attr.GetType() == typeof (TAttribute) || order >= 0) &&
                                             (condition == null || condition(attr))
                                         orderby -order
                                         select attr);
            return x.FirstOrDefault();
        }

        public static TAttribute GetCustomAttribute<TAttribute>(this MemberInfo self, bool inherit,
                                                                Func<TAttribute, bool> condition = null)
            where TAttribute : Attribute
        {
            if (self == null) return null;
            return self.GetCustomAttributes(typeof (TAttribute), inherit).InternalGet(condition);
        }

        public static TAttribute GetCustomAttribute<TAttribute>(this MemberInfo self,
                                                                Func<TAttribute, bool> condition = null)
            where TAttribute : Attribute
        {
            if (self == null) return null;
            return GetCustomAttribute(self, false, condition);
        }

        public static TAttribute GetCustomAttribute<TAttribute>(this Assembly self, bool inherit,
                                                                Func<TAttribute, bool> condition = null)
            where TAttribute : Attribute
        {
            if (self == null) return null;
            return self.GetCustomAttributes(typeof (TAttribute), inherit).InternalGet(condition);
        }

        public static TAttribute GetCustomAttribute<TAttribute>(this Assembly self,
                                                                Func<TAttribute, bool> condition = null)
            where TAttribute : Attribute
        {
            if (self == null) return null;
            return GetCustomAttribute(self, false, condition);
        }

        public static TAttribute GetCustomAttribute<TAttribute>(this Module self, bool inherit,
                                                                Func<TAttribute, bool> condition = null)
            where TAttribute : Attribute
        {
            if (self == null) return null;
            return self.GetCustomAttributes(typeof (TAttribute), inherit).InternalGet(condition);
        }

        public static TAttribute GetCustomAttribute<TAttribute>(this Module self,
                                                                Func<TAttribute, bool> condition = null)
            where TAttribute : Attribute
        {
            if (self == null) return null;
            return GetCustomAttribute(self, false, condition);
        }

        public static TAttribute GetCustomAttribute<TAttribute>(this ParameterInfo self, bool inherit,
                                                                Func<TAttribute, bool> condition = null)
            where TAttribute : Attribute
        {
            if (self == null) return null;
            return self.GetCustomAttributes(typeof (TAttribute), inherit).InternalGet(condition);
        }

        public static TAttribute GetCustomAttribute<TAttribute>(this ParameterInfo self,
                                                                Func<TAttribute, bool> condition = null)
            where TAttribute : Attribute
        {
            if (self == null) return null;
            return GetCustomAttribute(self, false, condition);
        }

        public static TAttribute GetCustomAttribute<TAttribute>(this Enum input, Func<TAttribute, bool> condition = null)
            where TAttribute : Attribute
        {
            if (input == null) return null;
            Type typ = input.GetType();
            string name = Enum.GetName(typ, input);
            FieldInfo field = typ.GetField(name, BindingFlags.Static | BindingFlags.Public);
            TAttribute attr = field.With(f => f.GetCustomAttribute(condition));
            return attr;
        }


        public static IEnumerable<Type> GetTypesWithAttribute<TAttribute, TBase>(this Assembly self,
                                                                                 Func<TAttribute, bool> condition = null)
            where TBase : class
            where TAttribute : Attribute
        {
            if (self == null) return null;
            return GetTypesWithAttribute<TAttribute, TBase>(self, false, condition);
        }

        public static IEnumerable<Type> GetTypesWithAttribute<TAttribute, TBase>(this Assembly self, bool inherit,
                                                                                 Func<TAttribute, bool> condition = null)
            where TBase : class
            where TAttribute : Attribute
        {
            if (self == null) return null;
            IEnumerable<Type> types =
                self.GetTypes().Where(t => t != typeof (TBase) && typeof (TBase).IsAssignableFrom(t));
            IEnumerable<Type> res = from Type t in types
                                    let a = t.GetCustomAttribute(inherit, condition)
                                    where a != null
                                    select t;
            return res;
        }

        public static IEnumerable<Type> GetTypesWithAttribute<TAttribute>(this Assembly self,
                                                                          Func<TAttribute, bool> condition = null)
            where TAttribute : Attribute
        {
            if (self == null) return null;
            return GetTypesWithAttribute(self, false, condition);
        }

        public static IEnumerable<Type> GetTypesWithAttribute<TAttribute>(this Assembly self, bool inherit,
                                                                          Func<TAttribute, bool> condition = null)
            where TAttribute : Attribute
        {
            if (self == null) return null;
            Type[] types = self.GetTypes();
            IEnumerable<Type> res = from Type t in types
                                    let a = t.GetCustomAttribute(inherit, condition)
                                    where a != null
                                    select t;
            return res;
        }


/*
        private static FieldInfo GetEventField(this IReflect type, string eventName)
        {
            var field = type.GetField(eventName, BindingFlags.Instance |
              BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.Public);

          

            if (field == null) return null; 

            if (field.FieldType == typeof(MulticastDelegate))  return field;

            if (field.FieldType.IsSubclassOf(typeof(MulticastDelegate))) return field; 

            return null;
        }

        public static IEnumerable<Delegate> GetEventInvocations(this object obj, string eventName)
        {
            var fi = obj.GetType().GetEventField(eventName);
            if (fi == null) return new Delegate[0];
            var d = fi.GetValue(obj) as MulticastDelegate;
            if (d == null) return new Delegate[0]; 
            return d.GetInvocationList();
        }

        public static void ClearEventInvocations(this object obj, string eventName)
        {
            var fi = obj.GetType().GetEventField(eventName);
            if (fi == null) return;
            fi.SetValue(obj, null);
        }
*/
    }
}