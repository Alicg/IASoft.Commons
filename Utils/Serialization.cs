using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.Serialization;
using System.Collections;
using System.Data.Objects;
using Utils.Extensions;

namespace Utils
{

    public static class SerializationExt
    {
        public static bool EliminationCycles(this object obj, List<object> exclude)
        {
            if (obj == null) return false;

            var type = obj.GetType();
            if (type.IsPrimitive || Reflection.IsTypeNullableByName(type.FullName) || type == typeof(Guid) || type == typeof(System.String) || type == typeof(DateTime) || type.IsEnum)
            {
                return false;
            }

            if (exclude == null || exclude.Count == 0) return false;

            if (obj is IEnumerable)
            {
                var remList = new List<object>();
                var lst = (obj as IEnumerable);
                foreach (var item in lst)
                {
                    if (EliminationCycles(item, exclude)) remList.Add(item);
                    //if (!(obj as IEnumerable).GetEnumerator().MoveNext()) break;
                }
                foreach (var item in remList) obj.GetType().InvokeMember("Remove", BindingFlags.InvokeMethod, null, obj, new object[] { item });
                return false;
            }

            var fnd = exclude.Where(a => ReferenceEquals(a, obj)).FirstOrDefault();
            if (fnd != null) return true;

            //locExclude.Add(obj);

            var pList = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public).Where(a => a.GetCustomAttributes(typeof(DataMemberAttribute), false).Length > 0);
            foreach (var pr in pList)
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
            foreach (var obj in list)
                EliminationCycles(obj, exclude);
        }

    }

}
