using System;
using Utils.Extensions;

namespace Utils
{
    /// <summary>
    /// обработка исключительных ситуация
    /// </summary>
    public class ExceptionHandling
    {
        private static ExceptionHandling _instance;

        public static ExceptionHandling Instance
        {
            get { return _instance ?? (_instance = new ExceptionHandling()); }
        }

        public string GetInnerExceptionMessage(Exception exc)
        {
            while (exc.InnerException != null)
            {
                exc = exc.InnerException;
            }
            return exc.Message;
        }

        public Exception GetInnerException(Exception exc)
        {
            if (exc.IsNull()) return null;
            while (exc.InnerException != null)
            {
                exc = exc.InnerException;
            }
            return exc;
        }

        public Exception GetInnerExceptionByType(Exception exc, Type innerExcType)
        {
            if (exc.IsNull()) return null;
            Exception inner = null;
            do
            {
                if (exc.GetType() == innerExcType) inner = exc;
                exc = exc.InnerException;
            } while (exc != null);
            return inner;
        }

        public string GetFullExceptionMessage(Exception exc)
        {
            string result = exc.Message;
            while (exc.InnerException != null)
            {
                result += exc.InnerException.Message + ";";
                exc = exc.InnerException;
            }
            return result;
        }

        public string GetExceptionMessageBySource(Exception exc, string sourceFilter)
        {
            while (exc != null)
            {
                if (exc.Source.Contains(sourceFilter)) return exc.Message;
                exc = exc.InnerException;
            }
            return null;
        }
    }
}