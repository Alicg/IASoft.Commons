using System;
using System.Threading.Tasks;

namespace Utils
{
    public static class AsyncUtils
    {
        public static async Task WaitWhile(Func<bool> predicate, TimeSpan intervalBetweenChecks)
        {
            while (predicate())
            {
                await Task.Delay(intervalBetweenChecks);
            }
        }
    }
}