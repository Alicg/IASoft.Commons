using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using Utils.Extensions;

namespace Utils
{
    /// <summary>
    /// работа со строками
    /// </summary>
    public class StringUtils
    {
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
            return quote + s.Replace(quote.ToString(), quote + quote.ToString()) + quote;
        }

        /// <summary>
        /// Dequotes string if it whole is quoted with quotes that is any char in <paramref name="quotes"/> string.<para></para>
        /// If not whole string is quoted or quotation is wrong - returns source string without any changing.<para></para>
        /// <example>
        /// <c>Example: </c>Input: s = 'string1''string2'''end -> Returns: string1'string2' and s = end
        /// </example>
        /// </summary>
        /// <param name="s">Source string</param>
        /// <param name="quotes">Quotes set</param>
        public static string DequotedStr(string s)
        {
            return DequotedStr(s, "\'\"");
        }

        /// <summary>
        /// Dequotes string if it whole is quoted with quotes that is any char in <paramref name="quotes"/> string.<para></para>
        /// If not whole string is quoted or quotation is wrong - returns source string without any changing.<para></para>
        /// <example>
        /// <c>Example: </c>Input: s = 'string1''string2'''end -> Returns: string1'string2' and s = end
        /// </example>
        /// </summary>
        /// <param name="s">Source string</param>
        /// <param name="quotes">Quotes set</param>
        public static string DequotedStr(string s, string quotes)
        {
            string str = s;
            string res = DequotedStr(ref str, quotes);
            return str == "" ? res : s;
        }

        /// <summary>
        /// Dequotes string if it whole is quoted with quotes that is any char in <paramref name="quotes"/> string.<para></para>
        /// If not whole string is quoted or quotation is wrong - returns source string without any changing.<para></para>
        /// <example>
        /// <c>Example: </c>Input: s = 'string1''string2'''end -> Returns: string1'string2' and s = end
        /// </example>
        /// </summary>
        /// <param name="s">Source string</param>
        /// <param name="quotes">Quotes set</param>
        public static string DequotedStr(ref string s)
        {
            return DequotedStr(ref s, "\"'");
        }

        /// <summary>
        /// Dequotes string if it whole is quoted with quotes that is any char in <paramref name="quotes"/> string.<para></para>
        /// If not whole string is quoted or quotation is wrong - returns source string without any changing.<para></para>
        /// <example>
        /// <c>Example: </c>Input: s = 'string1''string2'''end -> Returns: string1'string2' and s = end
        /// </example>
        /// </summary>
        /// <param name="s">Source string</param>
        /// <param name="quotes">Quotes set</param>
        public static string DequotedStr(ref string s, string quotes)
        {
            string res = s.Trim();
            s = "";
            if (res.Length < 2 || quotes.IndexOf(res[0]) == -1) return res;
            var sb = new StringBuilder();
            char quote = res[0];
            string ss = res.Substring(1);
            int idx = 0;
            do
            {
                while (idx < ss.Length && ss[idx] != quote)
                {
                    sb.Append(ss[idx++]);
                } // Searching quote
                if (idx >= ss.Length) return res;
                if (idx + 1 >= ss.Length || ss[idx + 1] != quote)
                {
                    s = ss.Substring(idx + 1);
                    return sb.ToString();
                }
                // Next char is quote 
                sb.Append(quote);
                idx += 2;
            } while (idx < ss.Length);
            return res;
        }


        public static string GetTransliterationFromUkrainianToEnglish(
            string paramString)
        {
            var locSymbolsArray = new string[2][]
                {
                    new string[72]
                        {
                            "а", "б", "в", "г", "д", "е", "ё", "ж", "з", "и", "й", "к", "л", "м", "н", "о", "п", "р", "с",
                            "т", "у", "ф", "х", "ц", "ч", "ш", "щ", "ъ", "ы", "ь", "э", "ю", "я", "і", "ї", "є", "А",
                            "Б", "В", "Г", "Д", "Е", "Ё", "Ж", "З", "И", "Й", "К", "Л", "М", "Н", "О", "П", "Р", "С",
                            "Т", "У", "Ф", "Х", "Ц", "Ч", "Ш", "Щ", "Ъ", "Ы", "Ь", "Э", "Ю", "Я", "І", "Ї", "Є"
                        },
                    new string[72]
                        {
                            "a", "b", "v", "g", "d", "e", "je", "zh", "z", "i", "j", "k", "l", "m", "n", "o", "p", "r", "s"
                            , "t", "u", "f", "kh", "tc", "ch", "sh", "sch", "", "i", "", "e", "ju", "ja", "i", "ji", "e"
                            , "A", "B", "V", "G", "D", "E", "Je", "Zh", "Z", "I", "J", "K", "L", "M", "N", "O", "P", "R"
                            , "S", "T", "U", "F", "Kh", "Tc", "Ch", "Sh", "Sch", "", "I", "", "E", "Ju", "Ja", "I", "Ji"
                            , "E"
                        }
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

        /// <summary>
        /// Returns part of the string till separator with checking amd ignoring (optional) separators in specified brackets.
        /// </summary>
        /// <param name="str">Source string</param>
        /// <param name="separator">Separator char</param>
        /// <param name="pos">Position after separator in source string</param>
        /// <param name="strBrackets">String of bracket chars</param>
        private static string GetPartStr(string str, char separator, out int pos, string strBrackets)
        {
            pos = 0;
            str = str.Trim();
            if (str == "") return "";
            var sb = new StringBuilder();
            do
            {
                if (strBrackets.IndexOf(str[pos]) != -1)
                {
                    string s = str.Substring(pos);
                    string ss = DequotedStr(ref s, str[pos].ToString());
                    sb.Append(str[pos]).Append(ss).Append(str[pos]);
                    pos += ss.Length + 2;
                }
                else if (str[pos] != separator)
                    sb.Append(str[pos++]);
            } while (pos < str.Length && str[pos] != separator);
            return sb.ToString();
        }

        /// <summary>
        /// Separate text in string with specified separator char with skipping separator in brackets (' and ").
        /// Returns trimmed dequoted left part of string before separator and left in <paramref name="s"/> rest trimmed and dequoted part of string after separator.
        /// If string doesn't contains any separator - returns whole string and set <paramref name="s"/> to empty string.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string SeparateText(ref string s, char separator)
        {
            if (s == null)
                return null;
            int I;
            s = s.Trim();
            string res = GetPartStr(s, separator, out I, @"'""");
            s = s.RightPart(s.Length - I - 1).Trim().DequotedStr(@"'""");
            return res.Trim().DequotedStr(@"'""");
        }

        public static void AddText(ref string s, string text, string separator)
        {
            if (text != "")
                if (s == "") s = text;
                else s = s + separator + text;
        }

        public static string ObjectToSQLParam(object X)
        {
            if (X == null)
                return "null";
            else if (
                X.GetType() == typeof (byte) ||
                X.GetType() == typeof (short) ||
                X.GetType() == typeof (int) ||
                X.GetType() == typeof (long) ||
                X.GetType() == typeof (sbyte) ||
                X.GetType() == typeof (ushort) ||
                X.GetType() == typeof (uint) ||
                X.GetType() == typeof (ulong)
                )
                return X.AsString();
            else if (
                X.GetType() == typeof (double) ||
                X.GetType() == typeof (float) ||
                X.GetType() == typeof (decimal)
                )
                return X.AsString().Replace(',', '.');
            else if (X.GetType() == typeof (bool))
                return (bool) X ? "1" : "0";
            else if (X.GetType() == typeof (DateTime))
            {
                return ((DateTime) X).ToString("F");
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
                    string s = p.Key.AsString();
                    if (s == "") continue;
                    if (s[0] == ':' || s[0] == '@')
                        queryText = queryText.ReplaceText(s, p.Value.AsSql());
                    else
                        queryText = queryText
                            .ReplaceText("{" + s + "}", p.Value.AsString())
                            .ReplaceText("@" + s, p.Value.AsSql())
                            .ReplaceText(":" + s, p.Value.AsSql());
                }
            }
            return queryText;
        }

        public static Bitmap DrawTextOnImage(string text, Color backColor, Color textColor, Font font, int width)
        {
            //first, create a dummy bitmap just to get a graphics object
            var img = new Bitmap(1, 1);
            var drawing = Graphics.FromImage(img);

            var sf = new StringFormat {Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center};
            
            //measure the string to see how big the image needs to be
            var textSize = drawing.MeasureString(text, font, width, sf);

            //free up the dummy image and old graphics object
            img.Dispose();
            drawing.Dispose();

            //create a new image of the right size
            img = new Bitmap((int)Math.Ceiling(textSize.Width), (int)Math.Ceiling(textSize.Height));

            drawing = Graphics.FromImage(img);

            //paint the background
            drawing.Clear(backColor);

            //create a brush for the text
            Brush textBrush = new SolidBrush(textColor);
            
            drawing.DrawString(text, font, textBrush, new RectangleF(new PointF(0, 0), textSize), sf);

            drawing.Save();

            textBrush.Dispose();
            drawing.Dispose();

            return img;
        }
    }
}