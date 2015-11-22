using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Utils.Extensions;

namespace Utils
{
    public static class Processes
    {
        /// <summary>
        /// Gets whether current process is x64
        /// </summary>
        /// <returns></returns>
        public static bool IsWow64
        {
            get { return Process.GetCurrentProcess().IsWow64(); }
        }

        private static string FindIndexedProcessName(int pid)
        {
            string processName = Process.GetProcessById(pid).ProcessName;
            Process[] processesByName = Process.GetProcessesByName(processName);
            string processIndexdName = null;

            for (int index = 0; index < processesByName.Length; index++)
            {
                processIndexdName = index == 0 ? processName : processName + "#" + index;
                var processId = new PerformanceCounter("Process", "ID Process", processIndexdName);
                if (processId.NextValue().As<int>().Equals(pid))
                {
                    return processIndexdName;
                }
            }

            return processIndexdName;
        }

        private static Process FindPidFromIndexedProcessName(string indexedProcessName)
        {
            var parentId = new PerformanceCounter("Process", "Creating Process ID", indexedProcessName);
            return Process.GetProcessById((int) parentId.NextValue());
        }

        public static Process Parent(Process process)
        {
            return FindPidFromIndexedProcessName(FindIndexedProcessName(process.Id));
        }

        #region - ApplicationType -

        /// <summary>
        /// Enum of application types
        /// </summary>
        public enum ApplicationTypes
        {
            Auto,
            WinForms,
            Console,
            Service
        }

        private static ApplicationTypes _applicationType = ApplicationTypes.Auto;

        /// <summary>
        /// Gets and implicit sets type of current process
        /// </summary>
        public static ApplicationTypes ApplicationType
        {
            get
            {
                return _applicationType == ApplicationTypes.Auto
                           ? Process.GetCurrentProcess().GetApplicationType()
                           : _applicationType;
            }
            set { _applicationType = value; }
        }

        /// <summary>
        /// Gets whether current process is console application
        /// </summary>
        public static bool IsConsole
        {
            get { return ApplicationType == ApplicationTypes.Console; }
        }

        /// <summary>
        /// Gets whether current process is windows service application
        /// </summary>
        public static bool IsService
        {
            get { return ApplicationType == ApplicationTypes.Service; }
        }

        #endregion

        public static List<int> GetFileLockProcessesIds(string path)
        {
            var tool = new Process();
            tool.StartInfo.FileName = "handle.exe";
            tool.StartInfo.Arguments = " -a \"{0}\"".Fmt(path);
            tool.StartInfo.UseShellExecute = false;
            tool.StartInfo.RedirectStandardOutput = true;
            tool.Start();
            tool.WaitForExit();
            var outputTool = tool.StandardOutput.ReadToEnd();
            var matchPattern = @"(?<=\s+pid:\s+)\b(\d+)\b(?=\s+)";
            return Regex.Matches(outputTool, matchPattern).Cast<Match>().Select(match => int.Parse(match.Value)).ToList();
        }

        public static List<Process> GetFileLockProcesses(string path)
        {
            var ids = GetFileLockProcessesIds(path);
            return ids.Select(Process.GetProcessById).ToList();
        }

        public static List<Process> GetModulesLockProcesses(string fullPath)
        {
            var res = new List<Process>();
            fullPath = fullPath.ToLower();
            foreach (var process in Process.GetProcesses())
            {
                try
                {
                    foreach (ProcessModule module in process.Modules)
                    {
                        if ((String.Compare(module.FileName.ToLower(), fullPath, StringComparison.Ordinal) == 0))
                        {
                            res.Add(process);
                            break;
                        }
                    }
                }
                catch(Exception)
                {
                    //Console.Write("\r\n" + exp.Message);
                }
            }
            return res;
        } 

    }
}
