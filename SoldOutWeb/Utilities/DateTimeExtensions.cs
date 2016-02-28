using System;
using System.Globalization;

namespace SoldOutWeb.Utilities
{
    static class DateTimeExtensions
    {
        private static GregorianCalendar _calendar = new GregorianCalendar();

        public static int GetWeek(this DateTime time)
        {
            return _calendar.GetWeekOfYear(time, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        }
    }
}