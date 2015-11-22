using System;
using System.ComponentModel;
using System.Data;
using System.Text;

namespace Utils.Extensions
{
    public static class DataRowExtension
    {
        #region Delegates

        public delegate void OnFieldValueEvent(DataColumn column, DataRow row, ref object value);

        #endregion

        #region TrimStringValue enum

        public enum TrimStringValue
        {
            NoTrim,
            TrimStart,
            TrimEnd,
            AllTrim
        }

        #endregion

        /// <summary>
        /// If DataTable has extended property of this name - its value sets TrimStringValues for string data of this table
        /// </summary>
        public const string TrimStringValueExtendedPropertyName = @"@@TrimStringValues";

        public static TrimStringValue TrimStringValues = TrimStringValue.NoTrim;

        /// <summary>
        /// Event is invoked after field value was read
        /// </summary>
        public static event OnFieldValueEvent OnGetFieldValue;

        /// <summary>
        /// Event is invoked before field value is written
        /// </summary>
        public static event OnFieldValueEvent OnSetFieldValue;

        public static string TrimDataRowValue(this string s, TrimStringValue? trimType = null)
        {
            trimType = trimType ?? TrimStringValues;
            TrimDelegate trim;
            switch (trimType)
            {
                case TrimStringValue.AllTrim:
                    trim = delegate(string str, char[] chars) { return str.Trim(chars); };
                    break;
                case TrimStringValue.TrimStart:
                    trim = delegate(string str, char[] chars) { return str.TrimStart(chars); };
                    break;
                case TrimStringValue.TrimEnd:
                    trim = delegate(string str, char[] chars) { return str.TrimEnd(chars); };
                    break;
                default:
                    trim = delegate(string str, char[] chars) { return str; };
                    break;
            }
            return trim(s, ' ', '\r', '\n', '\t');
        }

        /// <summary>
        /// Fill all DBNull-valued columns with default value of their type.
        /// </summary>
        public static void InitWithDefaultValues(this DataRow row, bool skipAllowDbNullColumns = false,
                                                 params Type[] skipColumnsOfThisTypes)
        {
            row.Table.Columns.ForEach<DataColumn>(col =>
                {
                    if (col.DataType == null) return;
                    if (row[col] == DBNull.Value
                        && (!skipAllowDbNullColumns || !col.AllowDBNull || col.DataType.NotIn(skipColumnsOfThisTypes)))
                        row[col] = col.DataType.DefaultValue();
                });
        }

        #region Get<T>

        public static T Get<T>(this DataRow r, [Localizable(false)] string columnName, T defValue = default(T))
        {
            if (r == null || r.RowState.In(DataRowState.Detached, DataRowState.Deleted)) return defValue;
            object res;
            DataColumn col = r.Table.Columns[columnName];
            if (col == null)
                return defValue;
            if (typeof (T) == typeof (byte[]) && col.DataType == typeof (string))
                res = Encoding.Unicode.GetBytes(r.Get<string>(columnName)).As(defValue);
            else if (typeof (T) == typeof (string) && col.DataType == typeof (byte[]))
                res = Encoding.Unicode.GetString(r.Get<byte[]>(columnName)).As(defValue);
            else
            {
                res = r[col].As(defValue);
                var trimType = r.Table.ExtendedProperties[TrimStringValueExtendedPropertyName].As<TrimStringValue?>();
                if (res.IsNotNull() && TrimStringValues != TrimStringValue.NoTrim && res is string)
                    res = res.AsString().TrimDataRowValue(trimType);
            }
            if (OnGetFieldValue != null)
                OnGetFieldValue(col, r, ref res);
            return res.As<T>();
        }

        public static object GetOfType(this DataRow r, Type type, [Localizable(false)] string columnName,
                                       object defValue = null)
        {
            if (r == null || r.RowState.In(DataRowState.Detached, DataRowState.Deleted)) return defValue;
            object res;
            DataColumn col = r.Table.Columns[columnName];
            if (type == typeof (byte[]) && col.DataType == typeof (string))
                res = Encoding.Unicode.GetBytes(r.Get<string>(columnName)).AsType(type, defValue);
            else if (type == typeof (string) && col.DataType == typeof (byte[]))
                res = Encoding.Unicode.GetString(r.Get<byte[]>(columnName)).As(defValue);
            else
            {
                res = r[col].AsType(type, defValue);
                var trimType = r.Table.ExtendedProperties[TrimStringValueExtendedPropertyName].As<TrimStringValue?>();
                if (res != null && TrimStringValues != TrimStringValue.NoTrim && res is string)
                    res = res.AsString().TrimDataRowValue(trimType);
            }
            if (OnGetFieldValue != null)
                OnGetFieldValue(col, r, ref res);
            return res;
        }

        #endregion

        #region Set<T>

        public static void Set<T>(this DataRow r, [Localizable(false)] string columnName, T value)
        {
            if (r == null || r.RowState.In(DataRowState.Detached, DataRowState.Deleted)) return;
            object val = value;
            DataColumn col = r.Table.Columns[columnName];
            if (typeof (T) == typeof (byte[]) && col.DataType == typeof (string))
                val = Encoding.Unicode.GetString(value.As<byte[]>());
            else if (typeof (T) != typeof (byte[]) && col.DataType == typeof (byte[]))
                val = Encoding.Unicode.GetBytes(value.As<string>());
            else if (value.IsNull())
                val = col.AllowDBNull ? DBNull.Value : col.DefaultValue;
            else if (col.DataType == typeof (string) && col.MaxLength > 0)
                val = value.AsString().LeftPart(col.MaxLength);

            if (OnSetFieldValue != null)
                OnSetFieldValue(col, r, ref val);
            r[col] = val;
        }

        #endregion

        #region Nested type: TrimDelegate

        private delegate string TrimDelegate(string s, params char[] chars);

        #endregion
    }
}