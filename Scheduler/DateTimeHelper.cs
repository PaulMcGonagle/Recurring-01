using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Scheduler
{
    public static class DateTimeHelper
    {
        public enum RollStrategyType
        {
            Back,
            Skip,
            Forward,
            Throw
        }

        public const RollStrategyType RollStrategyDefault = RollStrategyType.Skip;

        public static LocalDate GetToday()
        {
            var zone = NodaTime.TimeZones.BclDateTimeZone.ForSystemDefault();
            return SystemClock.Instance.Now.InZone(zone).LocalDateTime.Date;
        }

        public static LocalDateTime GetLocalDateTime(this IClock clock)
        {
            var zone = NodaTime.TimeZones.BclDateTimeZone.ForSystemDefault();
            return clock.Now.InZone(zone).LocalDateTime;
        }

        public static YearMonth GetLocalYearMonth(this IClock clock)
        {
            var localDateTime = GetLocalDateTime(clock);
            return localDateTime.ToYearMonth(); ;
        }

        public static LocalDate AddWeeks(this LocalDate dt, int Weeks)
        {
            return dt.PlusDays(Weeks * 7);
        }

        public static ZonedDateTime GetZonedDateTime(LocalDateTime ldt, string timeZoneProvider)
        {
            var london = DateTimeZoneProviders.Tzdb[timeZoneProvider];

            return london.AtStrictly(ldt);
        }

        public static ZonedDateTime GetZonedDateTime(LocalDate ld, LocalTime lt, string timeZoneProvider)
        {
            return GetZonedDateTime(ld.At(lt), timeZoneProvider);
        }

        public static int GetDaysInMonth(int year, int month)
        {
            var calendar = NodaTime.CalendarSystem.GetJulianCalendar(1);

            return calendar.GetDaysInMonth(year, month);
        }

        public static LocalDate GetLocalDate(int year, YearMonth.MonthValue month, int day)
        {
            return new LocalDate(year, (int)month, day);
        }

        public static YearMonth ToYearMonth(this LocalDateTime value)
        {
            return value.Date.ToYearMonth();
        }

        public static YearMonth ToYearMonth(this LocalDate value)
        {
            return new Scheduler.YearMonth() { LocalDate = value };
        }

        public static IEnumerable<LocalDate> Range(LocalDate start, int count, int interval = 1)
        {
            return Enumerable.Range(0, count).Select(i => start.PlusDays(i * interval));
        }

        public static IEnumerable<LocalDate> Range(LocalDate start, LocalDate end, int interval = 1)
        {
            var period = NodaTime.Period.Between(start, end, PeriodUnits.Days);

            var days = Convert.ToInt32((period.Days + 1) / interval);

            return Range(start, days, 1);
        }

        public static YearMonth.MonthValue ToMonthValue(this int month)
        {
            if (!typeof(YearMonth.MonthValue).IsEnumDefined(month))
                throw new ArgumentOutOfRangeException(string.Format("Invalid Month: {0}", month));

            return (YearMonth.MonthValue)Enum.Parse(typeof(YearMonth.MonthValue), month.ToString());
        }
    }
}
