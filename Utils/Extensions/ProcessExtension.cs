using System;
using System.Diagnostics;
using System.IO;
using System.Management;

namespace Utils.Extensions
{
    public static class ProcessExtensions
    {
        /// <summary>
        /// Gets parent process of current process
        /// </summary>
        public static Process Parent(this Process process)
        {
            using (
                var query = new ManagementObjectSearcher("root\\CIMV2",
                                                         "SELECT ParentProcessId FROM Win32_Process WHERE ProcessId = {0}"
                                                             .Fmt(process.Id)))
            {
                ManagementObjectCollection.ManagementObjectEnumerator results = query.Get().GetEnumerator();
                return !results.MoveNext() ? null : Process.GetProcessById(results.Current["ParentProcessId"].AsInt());
            }
        }

        /// <summary>
        /// Gets parent process of current process in format DOMAIN\USERNAME
        /// </summary>
        public static string Owner(this Process process)
        {
            using (
                var query = new ManagementObjectSearcher("root\\CIMV2",
                                                         "SELECT * FROM Win32_Process WHERE ProcessId = {0}".Fmt(
                                                             process.Id)))
            {
                foreach (ManagementObject obj in query.Get())
                {
                    var argList = new[] {string.Empty, string.Empty};
                    int returnVal = Convert.ToInt32(obj.InvokeMethod("GetOwner", argList));
                    if (returnVal == 0) return Path.Combine(argList[1], argList[0]);
                }
                return null;
            }
        }

        /// <summary>
        /// Determines type of specified application
        /// </summary>
        public static Processes.ApplicationTypes GetApplicationType(this Process process)
        {
//                if (process.MainWindowHandle != IntPtr.Zero)
            if (WinApi.GetStdHandle(-10) == IntPtr.Zero)
                return Processes.ApplicationTypes.WinForms;
            else if (process.Parent().ProcessName.SameText(@"services"))
                return Processes.ApplicationTypes.Service;
            else
            {
                try
                {
                    return Console.In != StreamReader.Null
                               ? Processes.ApplicationTypes.Console
                               : Processes.ApplicationTypes.WinForms;
                }
                catch
                {
                    return Processes.ApplicationTypes.WinForms;
                }
            }
        }

        /// <summary>
        /// Gets whether current process is console application
        /// </summary>
        public static bool IsConsole(this Process process)
        {
            return GetApplicationType(process) == Processes.ApplicationTypes.Console;
        }

        /// <summary>
        /// Gets whether current process is windows service application
        /// </summary>
        public static bool IsService(this Process process)
        {
            return GetApplicationType(process) == Processes.ApplicationTypes.Service;
        }


        /// <summary>
        /// Gets whether process is x64
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        public static bool IsWow64(this Process process)
        {
            bool retVal = false;
            WinApi.IsWow64Process(process.Handle, out retVal);
            return retVal;
        }
    }
}