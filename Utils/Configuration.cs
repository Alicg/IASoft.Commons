using System;
using System.Configuration;
using Utils.Extensions;

namespace Utils
{
    /// <summary>
    /// работа с параметрами конфигурации
    /// </summary>
    public class Configuration
    {
        #region - IsNull & Exists -

        public static bool IsNull(string paramName)
        {
            return FindConfigurationParameter(paramName).IsNull();
        }

        public static bool IsNullConnection(string paramName)
        {
            return FindConnectionParameter(paramName).IsNull();
        }

        public static bool Exists(string paramName)
        {
            return !IsNull(paramName);
        }

        public static bool ExistsConnection(string paramName)
        {
            return !IsNullConnection(paramName);
        }

        #endregion

        #region AsString

        public static string AsString(string paramName)
        {
            return FindConfigurationParameter(paramName);
        }

        public static string AsString(string paramName, string defValue)
        {
            return GetConfigurationParameter(paramName, defValue);
        }

        #endregion

        #region AsInt

        public static int AsInt(string paramName)
        {
            return FindConfigurationParameter(paramName).AsInt();
        }

        public static int AsInt(string paramName, int defValue)
        {
            return FindConfigurationParameter(paramName).AsInt(defValue);
        }

        #endregion

        #region AsDouble

        public static double AsDouble(string paramName)
        {
            return FindConfigurationParameter(paramName).AsDouble();
        }

        public static double AsDouble(string paramName, double defValue)
        {
            return FindConfigurationParameter(paramName).AsDouble(defValue);
        }

        #endregion

        #region AsFloat

        public static double AsFloat(string paramName)
        {
            return FindConfigurationParameter(paramName).AsFloat();
        }

        public static double AsFloat(string paramName, double defValue)
        {
            return FindConfigurationParameter(paramName).AsDouble(defValue);
        }

        #endregion

        #region AsBool

        public static bool AsBool(string paramName)
        {
            return FindConfigurationParameter(paramName).AsBool(false);
        }

        public static bool AsBool(string paramName, bool defValue)
        {
            return FindConfigurationParameter(paramName).AsBool(defValue);
        }

        #endregion

        #region AsDateTime

        public static DateTime AsDateTime(string paramName)
        {
            return FindConfigurationParameter(paramName).AsDateTime();
        }

        public static DateTime AsDateTime(string paramName, DateTime defValue)
        {
            return FindConfigurationParameter(paramName).AsDateTime(defValue);
        }

        #endregion

        #region AsDate

        public static DateTime AsDate(string paramName)
        {
            return FindConfigurationParameter(paramName).AsDate();
        }

        public static DateTime AsDate(string paramName, DateTime defValue)
        {
            return FindConfigurationParameter(paramName).AsDate(defValue);
        }

        #endregion

        #region AsTime

        public static TimeSpan AsTime(string paramName)
        {
            return FindConfigurationParameter(paramName).AsTime();
        }

        public static TimeSpan AsTime(string paramName, DateTime defValue)
        {
            return FindConfigurationParameter(paramName).AsTime(defValue);
        }

        #endregion

        #region - GetConfigurationParameter -

        public static string GetConfigurationParameter(string paramParameterName, string paramDefValue)
        {
            string locParamValue = FindConfigurationParameter(paramParameterName) ?? paramDefValue;
            return locParamValue;
        }

        public static string GetConfigurationParameter(string paramParameterName)
        {
            string locParamValue = GetConfigurationParameter(paramParameterName, "");
            if (locParamValue == null)
                throw new Exception(
                    "Отсутствует требуемый параметр конфигурации! Проверьте .config-файл приложения! Имя параметра='" +
                    paramParameterName + "'");
            return locParamValue;
        }

        public static string GetConnectionParameter(string paramParameterName)
        {
            ConnectionStringSettings locParamValue = ConfigurationManager.ConnectionStrings[paramParameterName];
            if (locParamValue == null || locParamValue.ConnectionString.AsString() == "")
                throw new Exception(
                    "Отсутствует требуемый параметр конфигурации! Проверьте .config-файл приложения! Имя параметра='" +
                    paramParameterName + "'");
            return locParamValue.ConnectionString;
        }

        #endregion

        #region - FindConnectionParameter -

        public static string FindConnectionParameter(string paramParameterName)
        {
            ConnectionStringSettings locSettings = ConfigurationManager.ConnectionStrings[paramParameterName];
            return locSettings == null ? null : locSettings.ConnectionString;
        }

        public static string FindConfigurationParameter(string paramParameterName)
        {
            string locParamValue = ConfigurationManager.AppSettings[paramParameterName];
            return locParamValue;
        }

        #endregion
    }
}