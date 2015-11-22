using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Utils.Extensions
{
    public static class ExceptionExtension
    {
        #region  - Exception extensions -

        #region - Error -

        public static void Error(this Exception exc)
        {
            Error(exc, exc.Message);
        }

        public static void Error(this Exception exc, string msg)
        {
            if (exc is AbortException) return;
            msg = ErrorLog.AnalizeErrorMessage(msg, exc);
            if (msg != "")
            {
                Messages.Error(msg);
            }
            if (ErrorLog.Params.NeedHalt)
                Process.GetCurrentProcess().Kill();
            ErrorLog.Params.Clear();
        }


        /// <summary>
        /// Returns all messages of the exception include all nested detail exceptions
        /// </summary>
        public static string FullMessage(this Exception e)
        {
            string iem = e.InnerExceptionMessage();
            return e.Message + (iem.IsEmpty() ? "" : ErrorLog.InnerExceptionSeparator + e.InnerExceptionMessage());
        }

        #endregion

        public static T GetInnerExceptionByType<T>(this Exception exc) where T : Exception
        {
            return (T) ExceptionHandling.Instance.GetInnerExceptionByType(exc, typeof (T));
        }

        public static Exception GetInnerException(this Exception exc)
        {
            return ExceptionHandling.Instance.GetInnerException(exc);
        }

        public static string GetMessageBySource(this Exception exc, string sourceFilter)
        {
            return ExceptionHandling.Instance.GetExceptionMessageBySource(exc, sourceFilter);
        }

        public static string GetInnerMessage(this Exception exc)
        {
            return ExceptionHandling.Instance.GetInnerExceptionMessage(exc);
        }

        public static string GetFullMessage(this Exception exc)
        {
            return ExceptionHandling.Instance.GetFullExceptionMessage(exc);
        }

        #region Specific exceptions
        
        #region cmdline exception
        /// <summary>
        /// метод для поиска пути к блокирующему компилятор файлу ..//..//*.cmlLine
        /// </summary>
        /// <param name="exc"></param>
        /// <param name="cmdLineFilePath">полный путь к файлу *.cmdLine</param>
        /// <returns>Является ли ошибка блокировкой компилятора файлом *.cmdLine</returns>
        public static bool IsCscCmdLineException(this Exception exc, out string cmdLineFilePath)
        {
            var excText = exc.GetFullMessage();
            cmdLineFilePath = null;
            var isCscCmdLineException = excText.Contains("csc.exe") && excText.Contains(".cmdline");
            if (isCscCmdLineException)
            {
                var cmdLineFilePathMatch = Regex.Match(excText, @"\w:\\Windows\\TEMP\\\w+\.cmdline");
                if (cmdLineFilePathMatch.Success)
                    cmdLineFilePath = cmdLineFilePathMatch.Value;
            }
            return isCscCmdLineException;
        }
        #endregion
        #endregion

        #endregion
    }
}