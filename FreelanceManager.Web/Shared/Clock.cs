using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace FreelanceManager.Web.Shared
{
    public class Clock
    {
        public DateTime Now
        {
            get
            {
                return DateTime.UtcNow;
            }
        }

        public int WeekNumber(DateTime time)
        {
            return 0;
        }

        public static int Iso8601WeekNumber(DateTime dt)
        {
            return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(dt, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        public static int GetWeekOfMonth(DateTime dt)
        {

            int weekOfYear = Iso8601WeekNumber(dt);

            if (dt.Month == 1)
            {
                //week of year == week of month in January
                //this also fixes the overflowing last December week
                return weekOfYear;
            }

            int weekOfYearAtFirst = Iso8601WeekNumber(dt.AddDays(1 - dt.Day));
            return weekOfYear - weekOfYearAtFirst + 1;
        }
    }
}