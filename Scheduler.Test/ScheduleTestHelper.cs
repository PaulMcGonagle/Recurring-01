﻿using System.Collections.Generic;
using NodaTime;
using NodaTime.Testing;

namespace Scheduler.Test
{
    public static class ScheduleTestHelper
    {
        public static readonly IEnumerable<IsoDayOfWeek> Weekdays =
            new List<IsoDayOfWeek>(new[]
                {
                    IsoDayOfWeek.Monday,
                    IsoDayOfWeek.Tuesday,
                    IsoDayOfWeek.Wednesday,
                    IsoDayOfWeek.Thursday,
                    IsoDayOfWeek.Friday
                });

        public static readonly IEnumerable<IsoDayOfWeek> WeekendDays =
            new List<IsoDayOfWeek>(new[]
                {
                    IsoDayOfWeek.Saturday,
                    IsoDayOfWeek.Sunday
                });

        public static readonly IEnumerable<IDate> BankHolidays =
            new List<IDate>(new[]
                {
                    new Date(2016, YearMonth.MonthValue.August, 29),
                    new Date(2017, YearMonth.MonthValue.December, 26),
                    new Date(2017, YearMonth.MonthValue.December, 27),
                    new Date(2017, YearMonth.MonthValue.January, 2),
                    new Date(2017, YearMonth.MonthValue.April, 14),
                    new Date(2017, YearMonth.MonthValue.April, 17),
                    new Date(2017, YearMonth.MonthValue.May, 1),
                    new Date(2017, YearMonth.MonthValue.May, 29),
                    new Date(2017, YearMonth.MonthValue.August, 28),
                    new Date(2017, YearMonth.MonthValue.December, 25),
                    new Date(2017, YearMonth.MonthValue.December, 26),
                });



        public static IClock GetFakeClock(int year, YearMonth.MonthValue month, int day, int hour = 0, int minute = 0, int second = 0)
        {
            return GetFakeClock(year, (int)month, day, hour, minute, second);
        }

        public static IClock GetFakeClock(int year, int month, int day, int hour = 0, int minute = 0, int second = 0)
        {
            return new FakeClock(Instant.FromUtc(year, month, day, hour, minute, second));
        }
    }
}
