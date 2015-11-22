using System;
using System.IO;

namespace Utils.Extensions
{
    public static class ApplicationExtension
    {
        public static bool IsConsole()
        {
            return Console.In != StreamReader.Null;
        }
    }
}