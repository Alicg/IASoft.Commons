using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Utils.Extensions
{
    [Localizable(true)]
    public static class StringExtension
    {
        #region IsEmpty/IsNotEmpty

        /// <summary>
        /// Checks whether string is null or empty.
        /// (for old framework versions)
        /// </summary>
        /// <param name="s">Testing string</param>
        public static bool IsEmpty(this string s)
        {
            return s.IsEmpty(false);
        }

        /// <summary>
        /// Checks whether string is null or empty.
        /// </summary>
        /// <param name="s">Testing string</param>
        /// <param name="trimString">Makes trim() before testing</param>
        public static bool IsEmpty(this string s, bool trimString = false)
        {
            if (trimString && s != null) s = s.Trim();
            return string.IsNullOrEmpty(s);
        }

        /// <summary>
        /// Checks whether string is not null or empty.
        /// (for old framework versions)
        /// </summary>
        /// <param name="s">Testing string</param>
        public static bool IsNotEmpty(this string s)
        {
            return s.IsNotEmpty(false);
        }

        /// <summary>
        /// Checks whether string is not null or empty.
        /// </summary>
        /// <param name="s">Testing string</param>
        /// <param name="trimString">Makes trim() before testing</param>
        public static bool IsNotEmpty(this string s, bool trimString = false)
        {
            if (trimString && s != null) s = s.Trim();
            return !string.IsNullOrEmpty(s);
        }

        #endregion

        #region LeftPart

        /// <summary>
        /// Returns left part of string defined length. If string length is less than parameter length - returns whole string.
        /// </summary>
        /// <param name="s">Source string</param>
        /// <param name="length">Length of the left part.</param>
        public static string LeftPart(this string s, int length)
        {
            if (length < 0 || s.IsEmpty()) return "";
            return length >= s.Length ? s : s.Substring(0, length);
        }

        #endregion

        #region RightPart

        /// <summary>
        /// Returns right part of string defined length. If string length is less than parameter length - returns whole string.
        /// </summary>
        /// <param name="s">Source string</param>
        /// <param name="length">Length of the right part.</param>
        public static string RightPart(this string s, int length)
        {
            if (length < 0) return "";
            return length >= s.Length ? s : s.Substring(s.Length - length, length);
        }

        #endregion

        #region SameText

        /// <summary>
        /// Compares two strings ignoring case according current culture.
        /// </summary>
        /// <param name="str1">First string - can be null</param>
        /// <param name="str2">Second string - can be null</param>
        public static bool SameText(this string str1, string str2)
        {
            return str1.AsString().Equals(str2.AsString(), StringComparison.CurrentCultureIgnoreCase);
        }

        #endregion

        /// <summary>
        /// Replaces the format item in a specified string with the string representation of a corresponding object in a specified array.
        /// </summary>
        /// <param name="format">A composite format string</param>
        /// <param name="args">An object array that contains zero or more objects to format</param>
        public static string Fmt(this string format, params object[] args)
        {
            return String.Format(format.AsString(), args);
        }

        public static string FmtQuery(this string str, Hashtable args)
        {
            return StringUtils.FmtQuery(str, args);
        }

        /// <summary>
        /// Duplicates specified string <paramref name="count"/> times.
        /// </summary>
        /// <param name="str">String to duplicate</param>
        /// <param name="count">Duplicate counter</param>
        public static string Duplicate(this string str, int count)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < count; i++)
                sb.Append(str);
            return sb.ToString();
        }

        public static string Md5(this string s)
        {
            var provider = new MD5CryptoServiceProvider();
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            var builder = new StringBuilder();

            bytes = provider.ComputeHash(bytes);

            foreach (byte b in bytes)
                builder.Append(b.ToString(@"x2").ToLower());

            return builder.ToString();
        }

        public static string ToBase64(this string s)
        {
            byte[] toEncodeAsBytes = Encoding.UTF8.GetBytes(s);
            return Convert.ToBase64String(toEncodeAsBytes);
        }

        public static string FromBase64(this string s)
        {
            byte[] fromEncodeAsBytes = Convert.FromBase64String(s);
            return Encoding.UTF8.GetString(fromEncodeAsBytes);
        }

        public static string AddText(this string str, string addStr, string separator)
        {
            if (str.IsEmpty()) return addStr;
            return str + separator + addStr;
        }

        public static string SeparateText(this string str, char separator)
        {
            return StringUtils.SeparateText(ref str, separator);
        }

        public static string SetCharAt(this string str, int index, char @char)
        {
            if (str.IsEmpty() || str.Length <= index)
                return str;
            return str.Substring(0, index) + @char + str.Substring(index + 1, str.Length - index - 1);
        }

        /// <summary>
        /// Replace chars in the string str. oldChars - set of chars to be replaced with appropriate char in newChars.
        /// <para>
        /// <example>
        /// str = "3a2q31", oldChars = '123', newChars = 'abc', result = "cabqca"
        /// </example>
        /// </para>
        /// </summary>
        public static string ReplaceChars(this string str, string oldChars, string newChars)
        {
            oldChars = oldChars.Substring(0, newChars.Length);
            char[] ch = str.ToCharArray();
            for (int i = 0; i < ch.Length; i++)
            {
                int idx = oldChars.IndexOf(ch[i]);
                if (idx >= 0)
                    ch[i] = newChars[idx];
            }
            return new string(ch);
        }


        public static string CutTail(this string s, string tail)
        {
            if (s.RightPart(tail.Length).SameText(tail))
                s = s.Substring(0, s.Length - tail.Length);
            return s;
        }

        #region To

        public static string ToLowCaseFirstChar(this string str)
        {
            return str.IsEmpty() ? str : str.Substring(0, 1).ToLower() + str.Substring(1);
        }

        public static string ToUpCaseFirstChar(this string str)
        {
            return str.IsEmpty() ? str : str.Substring(0, 1).ToUpper() + str.Substring(1);
        }

        public static string ToUpCaseOnlyFirstChar(this string str)
        {
            return str.IsEmpty() ? str : str.Substring(0, 1).ToUpper() + str.Substring(1).ToLower();
        }

        /// <summary>
        /// Преобразует строку вида key[delim1]value[delim2] в Dictionary<string,string>
        /// </summary>
        public static Dictionary<string, string> ToParametersDictionary(this string str, string delim1 = null,
                                                                        string delim2 = null)
        {
            delim1 = delim1 ?? "=";
            delim2 = delim2 ?? ";";
            var retDictionary = new Dictionary<string, string>();
            str.Split(new[] {delim2}, StringSplitOptions.RemoveEmptyEntries).ForEach(pair =>
                {
                    string[] arr = pair.Split(new[] {delim1}, StringSplitOptions.RemoveEmptyEntries);
                    retDictionary.Add(arr[0], arr[1]);
                });
            return retDictionary;
        }

        #endregion

        #region ConvertEncoding

        public static string ConvertEncoding(this string str, string sourceEncoding, string destEncoding)
        {
            return ConvertEncoding(str, Encoding.GetEncoding(sourceEncoding), Encoding.GetEncoding(destEncoding));
        }

        public static string ConvertEncoding(this string str, Encoding sourceEncoding, Encoding destEncoding)
        {
            return str.IsEmpty() ? str : destEncoding.GetString(sourceEncoding.GetBytes(str));
        }

        #endregion

        #region To ByteArray

        public static byte[] FromHex(this string hex)
        {
            hex = hex.Replace("-", "");
            hex = hex.Replace("0x", "");
            var raw = new byte[hex.Length/2];
            for (var i = 0; i < raw.Length; i++)
            {
                raw[i] = Convert.ToByte(hex.Substring(i*2, 2), 16);
            }
            return raw;
        }

        #endregion


        #region - Quote and dequote string -

        /// <summary>
        /// Quotes string with specified quote char
        /// </summary>
        public static string QuotedStr(this string s, char quote = '\'')
        {
            return quote + s.Replace(quote.ToString(), quote + quote.ToString()) + quote;
        }

        /// <summary>
        /// Dequotes string if it whole is quoted with quotes that is any char in <paramref name="quotes"/> string.<para></para>
        /// If not whole string is quoted or quotation is wrong - returns source string without any changing.<para></para>
        /// </summary>
        /// <param name="s">Source string</param>
        /// <param name="quotes">Quotes set</param>
        public static string DequotedStr(this string s, string quotes = "\'\"")
        {
            string str = s;
            string res = StringUtils.DequotedStr(ref str, quotes);
            return str == "" ? res : s;
        }

        #endregion

        #region - Encription / Decription -

        /// <summary>
        /// Encrypts string with DES
        /// </summary>
        /// <param name="str">Source string</param>
        /// <param name="salt">Secure salt value</param>
        /// <param name="ignoreError">If true - returns input string as result when error is raised</param>
        public static string Encrypt(this string str, string salt, bool ignoreError = false)
        {
            if (str.IsEmpty()) return "";
            try
            {
                byte[] dv = {0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF};
                byte[] byKey = Encoding.UTF8.GetBytes(salt.Substring(0, 8));
                var des = new DESCryptoServiceProvider();
                byte[] inputArray = Encoding.UTF8.GetBytes(str);
                var ms = new MemoryStream();
                var cs = new CryptoStream(ms, des.CreateEncryptor(byKey, dv), CryptoStreamMode.Write);
                cs.Write(inputArray, 0, inputArray.Length);
                cs.FlushFinalBlock();
                return Convert.ToBase64String(ms.ToArray());
            }
            catch
            {
                if (ignoreError) return str;
                throw;
            }
        }

        /// <summary>
        /// Decripts string with DES
        /// </summary>
        /// <param name="str">Source string</param>
        /// <param name="salt">Secure salt value, deffered when encription has made</param>
        /// <param name="ignoreError">If true - returns input string as result when error is raised</param>
        public static string Decrypt(this string str, string salt, bool ignoreError = false)
        {
            if (str.IsEmpty()) return "";
            try
            {
                byte[] dv = {0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF};
                byte[] bKey = Encoding.UTF8.GetBytes(salt.Substring(0, 8));
                var des = new DESCryptoServiceProvider();
                byte[] inputByteArray = Convert.FromBase64String(str);
                var ms = new MemoryStream();
                var cs = new CryptoStream(ms, des.CreateDecryptor(bKey, dv), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                Encoding encoding = Encoding.UTF8;
                return encoding.GetString(ms.ToArray());
            }
            catch
            {
                if (ignoreError) return str;
                throw;
            }
        }

        #endregion

        #region ReplaceText

        /// <summary>
        /// Заменяет подстроку в строке без учета регистра (используются регекспы)
        /// </summary>
        public static string ReplaceText(this string s, string oldValue, string newValue)
        {
            return Regex.Replace(s, Regex.Escape(oldValue), newValue, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Converts wildcard pattern to regex pattern
        /// </summary>
        /// <param name="pattern">Wild card pattern</param>
        /// <returns>Regex pattern</returns>
        public static string WildcardToRegex(this string pattern)
        {
            return "^" + Regex.Escape(pattern).Replace("\\*", ".*").Replace("\\?", ".") + "$";
        }

        /// <summary>
        /// Checks whether string is matched determines wildcard pattern. 
        /// </summary>
        /// <param name="input">Testing string</param>
        /// <param name="pattern">Wildcard pattern - can use * or ?</param>
        /// <param name="ignoreCase">Ignore case flag</param>
        public static bool IsMatch(this string input, string pattern, bool ignoreCase = false)
        {
            return Regex.IsMatch(input, pattern.WildcardToRegex(),
                                 ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);
        }

        public static bool Like(this string input, string pattern)
        {
            return input.IsMatch(pattern.ReplaceChars("%_", "*?"));
        }

        #endregion

        #region To HTML

        public static string Nl2Br(this string s)
        {
            return s.Replace("\r\n", @"<br />").Replace("\n", @"<br />");
        }

        public static string Tab2Html(this string s, int charCount = 4)
        {
            string tab = "";
            for (int i = 0; i < charCount; i++)
                tab += "&ensp;"; // Полукегельная шпация (пробел шириной в половину кегля шрифта)
            return s.Replace("\t", tab);
        }

        #endregion
    }
}