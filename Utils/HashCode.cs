using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Utils
{
    public class HashCode
    {
        public static byte[] CalcMd5HashBytes(byte[] bytes)
        {
            var md5 = MD5.Create();
            return md5.ComputeHash(bytes);
        }

        public static string CalcMd5HashString(byte[] bytes)
        {
            var md5 = MD5.Create();
            var byteHash = md5.ComputeHash(bytes);
            return byteHash.Select(b => b.ToString("x2")).Aggregate("", (total, cur) => total + cur).ToUpper();
        }
    }
}
