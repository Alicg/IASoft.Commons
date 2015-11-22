using System;
using System.ComponentModel;
using System.Diagnostics;
using Utils.Extensions;

namespace Utils
{
    [Localizable(true)]
    public static class Messages
    {
        #region - ApplicationType -

        private static Processes.ApplicationTypes _applicationType = Processes.ApplicationTypes.Auto;

        public static Processes.ApplicationTypes ApplicationType
        {
            get
            {
                if (_applicationType == Processes.ApplicationTypes.Auto)
                    _applicationType = Process.GetCurrentProcess().GetApplicationType();
                return _applicationType;
            }
            set { _applicationType = value; }
        }

        #endregion

        #region - Events -

        #region Delegates

        public delegate int OnMessageDelegate(string text, MessageType messageType);

        #endregion

        public enum MessageType
        {
            Error,
            Warning,
            Info
        };

        public static OnMessageDelegate OnMessage;

        #endregion

        #region - MessageBox wrapper -

        public static void Display(string text, MessageType messageType)
        {
            try
            {
                if (OnMessage != null)
                    OnMessage(text, messageType);
                else
                {
                    switch (ApplicationType)
                    {
                        case Processes.ApplicationTypes.Console:
                            Console.WriteLine(text);
                            break;
                        case Processes.ApplicationTypes.Service:
                            break;
                    }
                }
            }
            catch
            {
            }
        }

        #endregion

        public static void Error(string msg)
        {
            Display(msg, MessageType.Error);
        }

        public static void Warning(string msg)
        {
            Display(msg, MessageType.Warning);
        }

        public static void Info(string msg)
        {
            Display(msg, MessageType.Info);
        }
    }
}