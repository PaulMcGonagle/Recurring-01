using System.Collections.Generic;
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

        public static readonly IEnumerable<LocalDate> BankHolidays =
            new List<LocalDate>(new[]
                {
                    new LocalDate(year: 2016, month: 8, day: 29),
                    new LocalDate(year: 2016, month: 12, day: 26),
                    new LocalDate(year: 2016, month: 12, day: 27),
                    new LocalDate(year: 2017, month: 1, day: 2),
                    new LocalDate(year: 2017, month: 4, day: 14),
                    new LocalDate(year: 2017, month: 4, day: 17),
                    new LocalDate(year: 2017, month: 5, day: 1),
                    new LocalDate(year: 2017, month: 5, day: 29),
                    new LocalDate(year: 2017, month: 8, day: 28),
                    new LocalDate(year: 2017, month: 12, day: 25),
                    new LocalDate(year: 2017, month: 12, day: 26),
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
