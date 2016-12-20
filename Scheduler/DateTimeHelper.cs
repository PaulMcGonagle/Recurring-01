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

        public static Scheduler.Date GetToday(IClock clock)
        {
            var zone = NodaTime.TimeZones.BclDateTimeZone.ForSystemDefault();
            return new Scheduler.Date(clock.Now.InZone(zone).LocalDateTime.Date);
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

        public static Scheduler.Date AddWeeks(this Scheduler.Date dt, int weeks)
        {
            return new Date(dt.Value.PlusDays(weeks * 7));
        }

        public static ZonedDateTime GetZonedDateTime(LocalDateTime ldt, string timeZoneProvider)
        {
            var london = DateTimeZoneProviders.Tzdb[timeZoneProvider];

            return london.AtStrictly(ldt);
        }

        public static ZonedDateTime GetZonedDateTime(Scheduler.Date ld, LocalTime lt, string timeZoneProvider)
        {
            return GetZonedDateTime(ld.Value.At(lt), timeZoneProvider);
        }

        public static int GetDaysInMonth(int year, YearMonth.MonthValue month)
        {
            var calendar = CalendarSystem.GetJulianCalendar(1);

            return calendar.GetDaysInMonth(year, (int)month);
        }

        public static Scheduler.Date GetLocalDate(int year, YearMonth.MonthValue month, int day)
        {
            return new Scheduler.Date(year, month, day);
        }

        public static YearMonth ToYearMonth(this LocalDateTime value)
        {
            return new Scheduler.Date(value.Date).ToYearMonth();
        }

        public static YearMonth ToYearMonth(this Scheduler.Date value)
        {
            return new YearMonth { Date = value };
        }

        public static IEnumerable<Scheduler.Date> Range(Scheduler.Date start, int count, int interval = 1)
        {
            return Enumerable.Range(0, count).Select(i => new Scheduler.Date(start.Value.PlusDays(i * interval)));
        }

        public static IEnumerable<Scheduler.Date> Range(Scheduler.Date start, Scheduler.Date end, int interval = 1)
        {
            var period = Period.Between(start.Value, end.Value, PeriodUnits.Days);

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
