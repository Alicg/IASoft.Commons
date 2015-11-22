using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utils
{
    /// <summary>
    /// работа со строками
    /// </summary>
    public class Strings2
    {

        public static string GetListOfStringsAsStringSeparatedByNewLine(
            List<string> paramList
            )
        {
            return GetListOfStringsAsString(paramList, "\r\n");
        }

        public static string GetArrayOfIntAsStringSeparatedByCommas(
            int[] paramArray
            )
        {
            string result = "";
            if (paramArray.Length > 0)
            {
                result = paramArray[0].ToString();
                for (int i = 1; i < paramArray.Length; i++)
                {
                    result += "," + paramArray[i].ToString();
                }
            }
            return result;
        }

        public static string GetArrayOfDoubleAsStringSeparatedByCommas(
            double[] paramArray
            )
        {
            string result = "";
            if (paramArray.Length > 0)
            {
                result = paramArray[0].ToString();
                for (int i = 1; i < paramArray.Length; i++)
                {
                    result += "," + paramArray[i].ToString();
                }
            }
            return result;
        }

        public static string GetArrayOfStringsInQuotesAsStringSeparatedByCommas(
            string[] paramArray
            )
        {
            string result = "";
            if (paramArray.Length > 0)
            {
                for (int i = 0; i < paramArray.Length; i++)
                {
                    if (result.Length > 0) result += ",";
                    result += "'" + paramArray[i] + "'";
                }
            }
            return result;
        }

        public static string GetArrayOfStringsAsStringSeparatedByCommas(
            string[] paramArray
            )
        {
            string result = "";
            if (paramArray.Length > 0)
            {
                result = paramArray[0];
                for (int i = 1; i < paramArray.Length; i++)
                {
                    result += "," + paramArray[i];
                }
            }
            return result;
        }

        public static string GetListOfStringsAsString(
            List<string> paramList,
            string paramDelimiter)
        {
            string result = "";
            foreach (var item in paramList)
            {
                result += item + paramDelimiter;
            }
            return result;
        }

        public static List<int> GetListOfIntFromStringSeparatedByCommas(string paramString)
        {
            string[] locArray = paramString.Split(',');
            List<int> result = new List<int>();
            foreach (string s in locArray)
                result.Add(Convert.ToInt32(s));

            return result;
        }

        public static string GetToStringOrEmptyStringIfNull(object paramObj)
        {
            return ((paramObj == null) ? "" : paramObj.ToString());
        }

        public static string GetParametersHashtableAsString(Hashtable paramParametersHashtable)
        {
            string result = "";
            if (paramParametersHashtable != null)
            {
                foreach (string key in paramParametersHashtable.Keys)
                {
                    result += key + "=" + (paramParametersHashtable[key] ?? "[null]") + "\r\n";
                }
                // удаляю последний перенос строки
                if (!result.IsEmpty()) result = result.Remove(result.Length - 2);
            }
            return result;
        }

        /// <summary>
        /// Возвращает строку в одинарных кавычках с правильным удвоением символа кавычки внутри строки
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string QuotedStr(string s)
        {
            return QuotedStr(s, '\'');
        }
        /// <summary>
        /// Возвращает строку в указанных кавычках с правильным удвоением символа кавычки внутри строки
        /// </summary>
        /// <param name="s"></param>
        /// <param name="quote"></param>
        /// <returns></returns>
        public static string QuotedStr(string s, char quote)
        {
            return quote + s.Replace(quote.ToString(), quote.ToString() + quote.ToString()) + quote;
        }

        /// <summary>
        /// <para>Извлекает строку из кавычек, но только в том случае, если вся строка заключена в кавычки.</para>
        /// <para>Если строка не полностью заключена в кавычки или неправильно "закавычена" - просто возвращает исходную строку.</para>
        /// <para>Кавычками считаются символы одинарной и двойной кавычки.</para>
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string DequotedStr(string s)
        {
            return DequotedStr(s, "\'\"");
        }
        /// <summary>
        /// <para>Извлекает строку из кавычек, но только в том случае, если вся строка заключена в кавычки.</para>
        /// <para>Если строка не полностью заключена в кавычки или неправильно "закавычена" - просто возвращает исходную строку.</para>
        /// <para>Кавычками считаются либой из символов строки, переданной в параметре quotes.</para>
        /// </summary>
        /// <param name="s"></param>
        /// <param name="quotes"></param>
        /// <returns></returns>
        public static string DequotedStr(string s, string quotes)
        {
            var Str = s;
            var Res = DequotedStr(ref Str, quotes);
            if (Str == "") return Res; else return s;
        }
        /// <summary>
        /// <para>Извлекает строку из кавычек.</para>
        /// <para>Если неправильно "закавычена" - просто возвращает исходную строку.</para>
        /// <para>Пример: Передали: s = 'строка''подстрока'''конец  -> Получили: строка'подстрока' и s = конец</para>
        /// <para>Кавычками считаются символы одинарной и двойной кавычки.</para>
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string DequotedStr(ref string s)
        {
            return DequotedStr(ref s, "\"'");
        }
        /// <summary>
        /// <para>Извлекает строку из кавычек.</para>
        /// <para>Если неправильно "закавычена" - просто возвращает исходную строку.</para>
        /// <para>Пример: Передали: s = 'строка''подстрока'''конец  -> Получили: строка'подстрока' и s = конец</para>
        /// <para>Кавычками считаются либой из символов строки, переданной в параметре quotes.</para>
        /// </summary>
        /// <param name="s"></param>
        /// <param name="quotes"></param>
        /// <returns></returns>
        public static string DequotedStr(ref string s, string quotes)
        {
            var Res = s.Trim();
            s = "";
            char quote;
            if (Res.Length < 2 || quotes.IndexOf(Res[0]) == -1) return Res;
            var SB = new StringBuilder();
            quote = Res[0];
            var ss = Res.Substring(1);
            int Idx = 0;
            do
            {
                while (Idx < ss.Length && ss[Idx] != quote) { SB.Append(ss[Idx++]); } // Поиск кавычки
                // Если достигли конца цикла - конец (неверная строка в кавычках)
                if (Idx >= ss.Length) return Res;
                // Если следующий символ последний или не кавычка  - конец 
                if (Idx + 1 >= ss.Length || ss[Idx + 1] != quote) { s = ss.Substring(Idx + 1); return SB.ToString(); }
                // Следующий символ кавычка 
                SB.Append(quote); Idx += 2;
            } while (Idx < ss.Length);
            return Res;
        }

        public static string DeleteFromStringIllegalForADLoginSymbols(
            string paramString
            )
        {
            return paramString.Replace("'", "").Replace("`", "").Replace("\"", "");
        }

        public static string GetTransliterationFromUkrainianToEnglish(
            string paramString)
        {
            string[][] locSymbolsArray = new string[2][]{
                new string[72] {"а", "б", "в", "г", "д", "е", "ё",  "ж",  "з", "и", "й", "к", "л", "м", "н", "о", "п", "р", "с", "т", "у", "ф",  "х",  "ц",  "ч",  "ш",  "щ",  "ъ", "ы", "ь", "э",  "ю",  "я", "і",  "ї", "є", "А", "Б", "В", "Г", "Д", "Е",  "Ё",  "Ж", "З", "И", "Й", "К", "Л", "М", "Н", "О", "П", "Р", "С", "Т", "У", "Ф",  "Х",  "Ц",  "Ч",  "Ш",   "Щ", "Ъ", "Ы", "Ь", "Э",  "Ю",  "Я", "І",  "Ї", "Є"},
				new string[72] {"a", "b", "v", "g", "d", "e", "je", "zh", "z", "i", "j", "k", "l", "m", "n", "o", "p", "r", "s", "t", "u", "f", "kh", "tc", "ch", "sh", "sch",  "", "i",  "", "e", "ju", "ja", "i", "ji", "e", "A", "B", "V", "G", "D", "E", "Je", "Zh", "Z", "I", "J", "K", "L", "M", "N", "O", "P", "R", "S", "T", "U", "F", "Kh", "Tc", "Ch", "Sh", "Sch",  "", "I",  "", "E", "Ju", "Ja", "I", "Ji", "E"}
				};
            //--
            try
            {
                string result = "";
                bool locInserted = false;

                foreach (char locCurrentSymbol in paramString)
                {
                    for (int i = 0; i <= 71; i++)
                    {
                        if (locCurrentSymbol == Convert.ToChar(locSymbolsArray[0][i]))
                        {
                            result += locSymbolsArray[1][i];
                            locInserted = true;
                            break;
                        }
                    }
                    if (!locInserted) result += locCurrentSymbol;
                    locInserted = false;
                }

                return result;
            }
            catch
            {
                throw;
            }
        }

        public static bool IsContainElementStringListWithoutRegister(
            List<string> paramStringList,
            string paramString)
        {
            try
            {
                foreach (string locStringInList in paramStringList)
                {
                    if (locStringInList.ToLower() == paramString.ToLower())
                    {
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// добавить ведущие символы
        /// </summary>
        /// <param name="paramString"></param>
        /// <param name="paramLengthExitString"></param>
        /// <param name="paramSymbol"></param>
        /// <returns></returns>
        public static string AddLeadingSymbols(
            string paramString,
            int paramLengthExitString,
            string paramSymbol)
        {
            while (paramString.Length < paramLengthExitString)
            {
                paramString = paramSymbol + paramString;
            }
            return paramString;
        }

        /// <summary>
        /// Добавить ведущие пробелы
        /// </summary>
        /// <param name="paramString"></param>
        /// <param name="paramLengthExitString"></param>
        /// <returns></returns>
        public static string AddLeadingSpaces(
            string paramString,
            int paramLengthExitString)
        {
            return AddLeadingSymbols(
                paramString,
                paramLengthExitString,
                " ");
        }

        /// <summary>
        /// Сравнение двух строк без учета регистра
        /// </summary>
        public static bool SameText(string S1, string S2)
        {
            return String.Compare(S1, S2, true) == 0;
        }

        private static string GetPartStr(string S, char separator, out int Pos, string StrBrackets)
        {
            Pos = 0;
            var Res = ""; S = S.Trim(); if (S == "") return Res;
            var SB = new StringBuilder();
            do
            {
                if (StrBrackets.IndexOf(S[Pos]) != -1)
                {
                    var s = S.Substring(Pos);
                    var ss = DequotedStr(ref s, S[Pos].ToString());
                    SB.Append(S[Pos]).Append(ss).Append(S[Pos]);
                    Pos += ss.Length + 2;
                }
                else if (S[Pos] != separator)
                    SB.Append(S[Pos++]);
            } while (Pos < S.Length && S[Pos] != separator);
            return SB.ToString();
        }

        public static string SeparateText(ref string S, char separator)
        {
            int I;
            S = S.Trim();
            string Res = GetPartStr(S, separator, out I, "'\"");
            S = S.RightPart(S.Length - I - 1).Trim();
            return Res.Trim();
        }

        public static void AddText(ref string S, string Text, string separator)
        {
            if (Text != "")
                if (S == "") S = Text; else S = S + separator + Text;
        }

        public static string ObjectToSQLParam(object X)
        {
            if (X == null)
                return "null";
            else if (
                     X.GetType() == typeof(byte) ||
                     X.GetType() == typeof(short) ||
                     X.GetType() == typeof(int) ||
                     X.GetType() == typeof(long) ||
                     X.GetType() == typeof(sbyte) ||
                     X.GetType() == typeof(ushort) ||
                     X.GetType() == typeof(uint) ||
                     X.GetType() == typeof(ulong)
              )
                return X.AsString();
            else if (
                     X.GetType() == typeof(double) ||
                     X.GetType() == typeof(float) ||
                     X.GetType() == typeof(decimal)
              )
                return X.AsString().Replace(',', '.');
            else if (X.GetType() == typeof(bool))
                return (bool)X ? "1" : "0";
            else if (X.GetType() == typeof(DateTime))
            {
                return ((DateTime)X).ToString("F");
            }
            else
                return X.AsString().QuotedStr();
        }

        /// <summary>
        /// Может использоваться для преобразования параметризованного запроса в 
        /// RAW-текст запроса.
        /// Подстроки вида {имя_параметра} заменяются на текстовое представление значения
        /// этого параметра в том виде, как оно есть.
        /// Подстроки вида @имя_параметра и :имя_параметра заменяются на такие значения, чтобы
        /// запрос отработал корректно, как если бы он был параметризован, например,
        /// дробное число 123,12 подставится в виде 123.12, дата - в виде строки в одинарных кавычках,
        /// строковые параметры - в виде строки в кавычках, логические парамеры - в виде значений 1 и 0,
        /// значения null в виде строки Null.
        /// </summary>
        /// <param name="queryText"></param>
        /// <param name="queryParams"></param>
        /// <returns></returns>
        public static string FmtQuery(string queryText, Hashtable queryParams)
        {
            if (queryParams != null)
            {
                foreach (DictionaryEntry p in queryParams)
                {
                    var s = p.Key.AsString();
                    if (s == "") continue;
                    if (s[0] == ':' || s[0] == '@')
                        queryText = queryText.ReplaceText(s, p.Value.AsSQL());
                    else
                        queryText = queryText
                                    .ReplaceText("{" + s + "}", p.Value.AsString())
                                    .ReplaceText("@" + s, p.Value.AsSQL())
                                    .ReplaceText(":" + s, p.Value.AsSQL());
                }
            }
            return queryText;
        }

    }
}
