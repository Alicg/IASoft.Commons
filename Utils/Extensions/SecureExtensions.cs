namespace Utils.Common.Extensions
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security;

    public static class SecureExtensions
    {
        public static string ConvertToUnsecureString(this SecureString secureString)
        {
            if (secureString == null)
                throw new ArgumentNullException("secureString");

            var unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secureString);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }

        public static SecureString ConvertToSecureString(this string str)
        {
            if (str == null) 
                throw new ArgumentNullException("str");
            var sStr = new SecureString();
            Array.ForEach(str.ToCharArray(), sStr.AppendChar);
            return sStr;
        }
    }
}
