using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using Utils.Extensions;

namespace Utils
{
    public static class ErrorLog
    {
        public static string InnerExceptionSeparator = Environment.NewLine + Environment.NewLine; // Delimeter string between main and inner exceptions
        public static ErrorAnalizerParams Params = new ErrorAnalizerParams();

        public static string ExceptionSeparator =
            "\n----------------------------------------------------------------\n\n";

        public static string TrimExceptMsg(string msg)
        {
            return msg.Split(new[] {InnerExceptionSeparator}, StringSplitOptions.RemoveEmptyEntries)[0];
        }

        public static void ApplicationThreadException(object sender, ThreadExceptionEventArgs e)
        {
            if (e.Exception is AbortException) return;

            string exceptMsg = AnalizeErrorMessage(e.Exception);

            Messages.Error(exceptMsg);

            if (Params.NeedHalt)
                Process.GetCurrentProcess().Kill();

            Params.Clear();
        }

        private static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception)
            {
                if (e.ExceptionObject is AbortException) return;

                string exceptMsg = AnalizeErrorMessage(e.ExceptionObject as Exception);

                Messages.Error(exceptMsg);

                Params.Clear();
            }
        }

        #region - AnalizeErrorMessage -

        public static string AnalizeErrorMessage(string msg)
        {
            return AnalizeErrorMessage(msg, null, "");
        }

        public static string AnalizeErrorMessage(Exception e)
        {
            return AnalizeErrorMessage("", e, "");
        }

        public static string AnalizeErrorMessage(string msg, Exception e)
        {
            return AnalizeErrorMessage(msg, e, "");
        }

        public static string InnerExceptionMessage(this Exception e)
        {
            string innerExceptMsg = "";
            if (e != null)
            {
                Exception innerException = e.InnerException;
                while (innerException != null)
                {
                    if (innerException.Message != "")
                        innerExceptMsg += InnerExceptionSeparator + innerException.Message;
                    innerException = innerException.InnerException;
                }
            }
            return innerExceptMsg;
        }

        public static string AnalizeErrorMessage(string msg, Exception e, string addErrorInfo)
        {
            if (msg == "" && e == null)
                return "";

            string exceptStackTrace = "";
            string innerExceptMsg = e.InnerExceptionMessage();
            if (e != null)
            {
                Params.ExceptMsg = e.Message;
                exceptStackTrace = e.StackTrace;
/*
                var innerException = e.InnerException;
                while (innerException != null)
                {
                    if (innerException.Message != "")
                        innerExceptMsg += InnerExceptionSeparator + innerException.Message;
                    innerException = innerException.InnerException;
                }
*/
            }
            Params.Text = msg.Trim();
            Params.E = e;
            Params.NeedHalt = false;
            if (Params.Text == "") Params.Text = Params.ExceptMsg;
            Params.ExceptMsg += innerExceptMsg;
            Params.StackTrace = exceptStackTrace;
            Params.Host = Dns.GetHostName();

            try
            {
                if (Params.ErrorAnalizer != null)
                {
                    if (Debugger.IsAttached)
                        Params.ErrorAnalizer(Params);
                    else
                    {
                        var runThread = new Thread(Params.ErrorAnalizer);
                        runThread.SetApartmentState(ApartmentState.STA);
                        runThread.Start(Params);
                        runThread.Join(10000);
                        runThread.Abort();
                    }
                }
            }
            catch
            {
            }

            if (Params.Solution != "")
            {
                msg = (msg == "" ? "" : msg + ExceptionSeparator) + Params.Solution;
            }
            else if (e != null && msg != e.Message)
                msg = (msg == "" ? "" : msg + ExceptionSeparator) + e.Message;

            if (Params.NeedHalt)
                msg += ExceptionSeparator + "Abnormal program termination.";
            Params.SupportInfo = addErrorInfo + (addErrorInfo != "" ? "\n\n" : "" + Params.SupportInfo);
            return msg;
        }

        #endregion

        #region - InitParams -

        public static void InitParams(ParameterizedThreadStart errorAnalizer = null, int appId = 0, string appName = "",
                                      string userName = "")
        {
            Params.ErrorAnalizer = errorAnalizer;
            Params.AppId = appId;
            if (appName == "")
            {
                object attr =
                    Assembly.GetEntryAssembly().GetCustomAttributes(typeof (AssemblyProductAttribute), false).
                        FirstOrDefault();
                if (attr != null) appName = ((AssemblyProductAttribute) attr).Product;
                if (appName.IsEmpty())
                {
                    string fName = Process.GetCurrentProcess().MainModule.FileName;
                    FileVersionInfo fi = FileVersionInfo.GetVersionInfo(fName);
                    appName = fi.FileDescription.IsEmpty()
                                  ? Path.GetFileNameWithoutExtension(fName)
                                  : fi.FileDescription;
                }
            }
            Params.AppName = appName;
            Params.UserName = userName;
        }

        #endregion

        #region - Init -

        public static void Init(ParameterizedThreadStart errorAnalizer = null, int appId = 0, string appName = "",
                                string userName = "")
        {
            InitParams(errorAnalizer, appId, appName, userName);
//            Application.ThreadException -= ApplicationThreadException;
//            Application.ThreadException += ApplicationThreadException;
//            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;
        }

        #endregion
    }
}