using System;
using System.Linq;
using System.Management;
using Utils.Extensions;

namespace Utils.Management
{
    public static class ManagementUtils
    {
        private static readonly object SyncObj = new object();
        public static int? GetOptimalProcessorNode()
        {
            lock (SyncObj)
            {
                var searcher = new ManagementObjectSearcher("select Name,PercentProcessorTime from Win32_PerfFormattedData_PerfOS_Processor");
                var lowestCpu = searcher.Get()
                    .Cast<ManagementObject>()
                    .Select(mo => new
                    {
                        Name = mo["Name"],
                        Usage = mo["PercentProcessorTime"]
                    }
                    )
                    .OrderBy(v => v.Usage).FirstOrDefault();
                return lowestCpu == null ? (int?)null : (int)Math.Pow(2, lowestCpu.Name.AsInt());
            }
        }

        /// <summary>
        /// Создание экземпляра Win32_PerfFormattedData_PerfOS_Processor
        /// </summary>
        public static void Init()
        {
            lock (SyncObj)
            {
                var c = new ManagementClass("Win32_PerfFormattedData_PerfOS_Processor");
                c.GetInstances();
            }
        }
    }
}
