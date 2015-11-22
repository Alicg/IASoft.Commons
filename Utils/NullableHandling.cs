using System;

namespace Utils
{
    /// <summary>
    /// обработка переменных, которые могут содержать пустые значения
    /// </summary>
    public class NullableHandling
    {
        public static void CheckIfObjectIsNonEmptyAndNonDBEmptyString(object paramObject)
        {
            if (paramObject == null)
            {
                throw new ApplicationException("Нет значения (null)!");
            }
            if (paramObject == DBNull.Value)
            {
                throw new ApplicationException("Нет значения (DBNull)!");
            }
            if (paramObject.ToString() == "")
            {
                throw new ApplicationException("Пустая строка!");
            }
        }

        public static void CheckIfObjectIsNonEmptyAndNonDBEmpty(object paramObject)
        {
            if (paramObject == null)
            {
                throw new ApplicationException("Нет значения (null)!");
            }
            if (paramObject == DBNull.Value)
            {
                throw new ApplicationException("Нет значения (DBNull)!");
            }
        }

        public static string GetDoubleLikeStringOrNULLIfZero(
            double paramObject)
        {
            return (paramObject != 0) ? paramObject.ToString() : "null";
        }

        public static string GetStringOrEmptyStringIfDBNull(
            object paramObject)
        {
            return (paramObject != DBNull.Value) ? paramObject.ToString() : "";
        }

        public static string GetStringOrNullIfDBNull(
            object paramObject)
        {
            return (paramObject != DBNull.Value) ? paramObject.ToString() : null;
        }


        public static string GetStringOrEmptyStringIfNull(
            object paramObject)
        {
            return (paramObject != null) ? paramObject.ToString() : "";
        }

        public static Int32 GetInt32OrZeroIfNull(
            object paramObject)
        {
            return (paramObject != null) ? Convert.ToInt32(paramObject) : 0;
        }

        public static Int32 GetInt32OrZeroIfDBNull(
            object paramObject)
        {
            return (paramObject != DBNull.Value) ? Convert.ToInt32(paramObject) : 0;
        }

        public static int? GetInt32OrNullIfDBNull(
            object paramObject)
        {
            if (paramObject != DBNull.Value)
            {
                return Convert.ToInt32(paramObject);
            }
            else
            {
                return null;
            }
        }

        public static int? GetInt32OrNullIfNull(
            object paramObject)
        {
            if (paramObject != null)
            {
                return Convert.ToInt32(paramObject);
            }
            else
            {
                return null;
            }
        }

        public static double GetDoubleOrZeroIfNull(
            object paramObject)
        {
            return (paramObject != null) ? Convert.ToDouble(paramObject) : 0;
        }

        public static Double GetDoubleOrZeroIfDBNull(
            object paramObject)
        {
            return (paramObject != DBNull.Value) ? Convert.ToDouble(paramObject) : 0;
        }

        public static double? GetDoubleOrNullIfDBNull(
            object paramObject)
        {
            if (paramObject != DBNull.Value)
            {
                return Convert.ToDouble(paramObject);
            }
            else
            {
                return null;
            }
        }


        public static DateTime GetDateOrMinimalDateIfDBNull(
            object paramObject)
        {
            DateTime D = (paramObject != DBNull.Value) ? Convert.ToDateTime(paramObject) : DateTime.MinValue;
            return DateTime.SpecifyKind(D, DateTimeKind.Unspecified);
        }

        public static DateTime GetDateOrCurrentDateIfDBNull(
            object paramObject)
        {
            DateTime D = (paramObject != DBNull.Value) ? Convert.ToDateTime(paramObject) : DateTime.Now;
            return DateTime.SpecifyKind(D, DateTimeKind.Unspecified);
        }

        public static Guid GetGuidOrZeroGuidIfDBNull(
            object paramObject)
        {
            return (paramObject != DBNull.Value) ? (Guid) paramObject : Guid.Empty;
        }

        public static Char GetCharOrSpaceCharIfDBNull(
            object paramObject)
        {
            return (paramObject != DBNull.Value) ? Convert.ToChar(paramObject) : ' ';
        }

        public static Decimal GetDecimalOrZeroIfDBNull(
            object paramObject)
        {
            return (paramObject != DBNull.Value) ? Convert.ToDecimal(paramObject) : 0;
        }

        public static bool GetBoolOrFalseIfDBNull(
            object paramObject)
        {
            return (paramObject != DBNull.Value) ? Convert.ToBoolean(paramObject) : false;
        }

        public static byte[] GetByteArrayOrNullIfDBNull(
            object paramObject)
        {
            return (paramObject != DBNull.Value) ? (byte[]) paramObject : null;
        }

        public static object GetValueOrNull(object paramObject, object paramNullValue)
        {
            return (paramObject != paramNullValue) ? paramObject : null;
        }

        public static object GetValueOrNullIfNullOrZero(double? paramObject)
        {
            double locValue = 0;
            if (paramObject != null) locValue = Convert.ToDouble(paramObject);
            return GetValueOrNullIfZero(locValue);
        }

        public static object GetValueOrNullIfZero(double paramObject)
        {
            return GetValueOrNull(paramObject, 0);
        }

        public static object GetValueOrNullIfMinusOne(double paramObject)
        {
            return GetValueOrNull(paramObject, -1);
        }


        public static object GetValueOrNull(double paramObject, double paramNullValue)
        {
            return (paramObject != paramNullValue) ? (object) paramObject : null;
        }

        public static object GetValueOrNullIfZero(int paramObject)
        {
            return GetValueOrNull(paramObject, 0);
        }

        public static object GetValueOrNullIfMinusOne(int paramObject)
        {
            return GetValueOrNull(paramObject, -1);
        }

        public static object GetValueOrNull(int paramObject, int paramNullValue)
        {
            return (paramObject != paramNullValue) ? (object) paramObject : null;
        }
    }
}