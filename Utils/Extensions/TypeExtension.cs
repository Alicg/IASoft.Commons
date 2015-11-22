using System;

namespace Utils.Extensions
{
    public static class TypeExtension
    {
        /// <summary>
        /// Creates instance of type default value.
        /// For string return empty string - NOT null
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static object DefaultValue(this Type self)
        {
            if (self == typeof (string)) return "";
            if (self.IsArray)
                return Activator.CreateInstance(self, 0);
            return self.IsValueType ? Activator.CreateInstance(self) : null;
        }

        /// <summary>
        /// Checks the type is one of integer types (byte, int, long, etc.)
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool IsInteger(this Type self)
        {
            return (self.IsValueType && self.In(
                typeof (sbyte),
                typeof (short),
                typeof (int),
                typeof (long),
                typeof (byte),
                typeof (ushort),
                typeof (uint),
                typeof (ulong)
                                            ));
        }

        /// <summary>
        /// Checks the type is one of numeric types (float, double, decimal)
        /// </summary>
        public static bool IsNumeric(this Type self)
        {
            return (self.IsValueType && self.In(
                typeof (float),
                typeof (double),
                typeof (decimal)
                                            ));
        }

        /// <summary>
        /// Checks the type is one of integer or numeric types (int, byte, float, double, decimal, etc.)
        /// </summary>
        public static bool IsNumber(this Type self)
        {
            return IsInteger(self) || IsNumeric(self);
        }

        /// <summary>
        /// Checks if specified type Nullable, ex. int?, DateTime?
        /// </summary>
        public static bool IsNullableType(this Type type)
        {
            return Nullable.GetUnderlyingType(type) != null;
        }

        /// <summary>
        /// Returns generic base type from Nullable: int? -> int, DateTime? -> DateTime
        /// </summary>
        public static Type GetNonNullableType(this Type type)
        {
            return type.IsNullableType() ? type.GetGenericArguments()[0] : type;
        }


        /// <summary>
        /// Checks if specified type could assign null values (all reference types and Nullable types)
        /// </summary>
        public static bool CanBeNull(this Type self)
        {
            return self.IsClass || self.IsNullableType();
        }
    }
}