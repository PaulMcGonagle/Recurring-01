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

        public static Date GetToday(IClock clock)
        {
            var zone = NodaTime.TimeZones.BclDateTimeZone.ForSystemDefault();
            return new Date(clock.Now.InZone(zone).LocalDateTime.Date);
        }

        public static LocalDateTime GetLocalDateTime(this IClock clock)
        {
            var zone = NodaTime.TimeZones.BclDateTimeZone.ForSystemDefault();
            return clock.Now.InZone(zone).LocalDateTime;
        }

        public static YearMonth GetLocalYearMonth(this IClock clock)
        {
            var localDateTime = GetLocalDateTime(clock);
            return localDateTime.ToYearMonth();
        }

        public static Date AddWeeks(this Date dt, int weeks)
        {
            return new Date(dt.Value.PlusDays(weeks * 7));
        }

        public static ZonedDateTime GetZonedDateTime(LocalDateTime ldt, string timeZoneProvider)
        {
            var london = DateTimeZoneProviders.Tzdb[timeZoneProvider];

            return london.AtStrictly(ldt);
        }

        public static ZonedDateTime GetZonedDateTime(IDate ld, LocalTime lt, string timeZoneProvider)
        {
            return GetZonedDateTime(ld.Value.At(lt), timeZoneProvider);
        }

        public static int GetDaysInMonth(int year, YearMonth.MonthValue month)
        {
            var calendar = CalendarSystem.GetJulianCalendar(1);

            return calendar.GetDaysInMonth(year, (int)month);
        }

        public static Date GetLocalDate(int year, YearMonth.MonthValue month, int day)
        {
            return new Date(year, month, day);
        }

        public static LocalDate GetNextWeekday(LocalDate input, IsoDayOfWeek requiredDayOfWeek, bool rollBack = false)
        {
            int startOffset;

            if (rollBack)
            {
                startOffset = requiredDayOfWeek - input.IsoDayOfWeek - (requiredDayOfWeek >= input.IsoDayOfWeek ? 0 : -7);
            }
            else
            {
                startOffset = requiredDayOfWeek - input.IsoDayOfWeek + (requiredDayOfWeek >= input.IsoDayOfWeek ? 0 : 7);
            }

            return input.PlusDays(startOffset);
        }

        public static YearMonth ToYearMonth(this LocalDateTime value)
        {
            return new Date(value.Date).ToYearMonth();
        }

        public static YearMonth ToYearMonth(this Date value)
        {
            return new YearMonth { Date = value };
        }

        public static IEnumerable<IDate> Range(Date start, int count, int interval = 1)
        {
            return new List<IDate>(Enumerable.Range(0, count).Select(i => new Date(start.Value.PlusDays(i * interval))));
        }

        public static IEnumerable<IDate> Range(Date start, Date end, int interval = 1)
        {
            var period = Period.Between(start.Value, end.Value, PeriodUnits.Days);

            var days = Convert.ToInt32((period.Days + 1) / interval);

            return Range(start, days);
        }

        public static YearMonth.MonthValue ToMonthValue(this int month)
        {
            if (!typeof(YearMonth.MonthValue).IsEnumDefined(month))
                throw new ArgumentOutOfRangeException(string.Format("Invalid Month: {0}", month));

            return (YearMonth.MonthValue)Enum.Parse(typeof(YearMonth.MonthValue), month.ToString());
        }
    }
}
