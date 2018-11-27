using System;

namespace SVA.Infrastructure.Collections
{
    public class DateToGroupHeaderConverter
    {
        public static GroupHeaderTypes Convert(DateTime elementDate)
        {
            var today = DateTime.Today;
            if (elementDate.Year == today.Year && elementDate.Month == today.Month && elementDate.Day == today.Day)
            {
                return GroupHeaderTypes.Today;
            }
            if (elementDate.Year == today.Year && elementDate.Month == today.Month && elementDate.Day == today.Day - 1)
            {
                return GroupHeaderTypes.Yesterday;
            }
            var dayDiff = today - elementDate;
            if (dayDiff.TotalDays < 7 && today.DayOfWeek > elementDate.DayOfWeek)
            {
                return GroupHeaderTypes.ThisWeek;
            }
            if (elementDate.Month == today.Month)
            {
                return GroupHeaderTypes.ThisMonth;
            }
            if (elementDate.Month == today.Month - 1)
            {
                return GroupHeaderTypes.LastMonth;
            }
            if (elementDate.Year == today.Year)
            {
                return GroupHeaderTypes.ThisYear;
            }
            if (elementDate.Year == today.Year - 1)
            {
                return GroupHeaderTypes.LastYear;
            }
            return GroupHeaderTypes.LongAgo;
        }

        public static DatesInterval ConvertBack(GroupHeaderTypes groupHeaderType)
        {
            var endOfToday = DateTime.MaxValue;
            var today = DateTime.Today;

            var endOfYesterday = today.AddMilliseconds(-1);
            var yesterday = today.AddDays(-1);

            var endOfThisWeek = yesterday.AddMilliseconds(-1);
            var thisWeek = today.AddDays(-1 * (int)today.DayOfWeek);

            var endOfThisMonth = thisWeek.AddMilliseconds(-1);
            var thisMonth = new DateTime(today.Year, today.Month, 1);

            var endOfLastMonth = thisMonth.AddMilliseconds(-1);
            var lastMonth = thisMonth.AddMonths(-1);

            var endOfThisYear = lastMonth.AddMilliseconds(-1);
            var thisYear = new DateTime(today.Year, 1, 1);

            var endOfLastYear = thisYear.AddMilliseconds(-1);
            var lastYear = new DateTime(today.Year - 1, 1, 1);

            var endOfLongAgo = lastYear.AddMilliseconds(-1);
            var longAgo = DateTime.MinValue;

            switch (groupHeaderType)
            {
                case GroupHeaderTypes.All:
                {
                    return new DatesInterval(DateTime.MinValue, DateTime.MaxValue);
                }
                case GroupHeaderTypes.Today:
                {
                    return new DatesInterval(today, endOfToday);
                }
                case GroupHeaderTypes.Yesterday:
                {
                    return new DatesInterval(yesterday, endOfYesterday);
                }
                case GroupHeaderTypes.ThisWeek:
                {
                    return new DatesInterval(thisWeek, endOfThisWeek);
                }
                case GroupHeaderTypes.ThisMonth:
                {
                    return new DatesInterval(thisMonth, endOfThisMonth);
                }
                case GroupHeaderTypes.LastMonth:
                {
                    return new DatesInterval(lastMonth, endOfLastMonth);
                }
                case GroupHeaderTypes.ThisYear:
                {
                    return new DatesInterval(thisYear, endOfThisYear);
                }
                case GroupHeaderTypes.LastYear:
                {
                    return new DatesInterval(lastYear, endOfLastYear);
                }
                case GroupHeaderTypes.LongAgo:
                {
                    return new DatesInterval(longAgo, endOfLongAgo);
                }
                default:
                {
                    throw new NotSupportedException("Not supported group header.");
                }
            }
        }

        public struct DatesInterval
        {
            public DatesInterval(DateTime intervalFrom, DateTime intervalTo)
            {
                this.IntervalFrom = intervalFrom;
                this.IntervalTo = intervalTo;
            }

            public DateTime IntervalFrom { get; }

            public DateTime IntervalTo { get; }

            public bool InInterval(DateTime dateToCheck)
            {
                return dateToCheck >= this.IntervalFrom && dateToCheck <= this.IntervalTo;
            }
        }
    }
}