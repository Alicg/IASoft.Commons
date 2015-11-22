using System;

namespace Utils.Extensions
{
    public static class DateTimeExtension
    {
        public static DateTime BeginOfMonth(this DateTime self)
        {
            return new DateTime(self.Year, self.Month, 1);
        }

        public static DateTime EndOfMonth(this DateTime self)
        {
            return self.BeginOfMonth().AddMonths(1).AddDays(-1);
        }
    }
}