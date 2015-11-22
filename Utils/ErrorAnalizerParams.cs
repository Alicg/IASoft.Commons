using System;
using System.Threading;

namespace Utils
{
    public class ErrorAnalizerParams
    {
        public int AppId;
        public string AppName = "";
        public Exception E;
        public ParameterizedThreadStart ErrorAnalizer;
        public string Text = "";
        public string UserName = "";

        #region - эти параметры не нужно заполнять - они устанавливаются сами -

        public int ErrorId;
        public string ExceptMsg = "";
        public string HelpUrl = "";
        public string Host = "";
        public bool NeedHalt;
        public string Solution = "";
        public int SolutionId;
        public string StackTrace = "";
        public string SupportInfo = "";

        #endregion

        public void Clear()
        {
            Solution = "";
            SupportInfo = "";
            HelpUrl = "";
            SolutionId = 0;
            ErrorId = 0;
            ExceptMsg = "";
            StackTrace = "";
            Host = "";
            E = null;
            Text = "";
        }

        public bool IsEmpty()
        {
            return Host == "" && SupportInfo == "";
        }
    }
}