using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using Scheduler.ScheduleAbstracts;

namespace Scheduler
{
    public class YearMonth
    {
        public enum MonthValue
        {
            January = 1,
            February,
            March,
            April,
            May,
            June,
            July,
            August,
            September,
            October,
            November,
            December,
        }

        public class YearLimit
        {
            public const int Lower = 1000;
            public const int Upper = 9999;
        }

        private int _year;

        public int Year
        {
            set
            {
                if (value < 1000 || value > 9999)
                    throw new ArgumentOutOfRangeException($"Invalid Year value: {value}");

                _year = value;
            }
            get
            {
                return _year;
            }
        }

        public MonthValue Month { set; get; }

        public Date Date
        {
            set
            {
                Year = value.Value.Year;
                Month = value.Value.Month.ToMonthValue();
            }
        }


        public static IEnumerable<YearMonth> Range(YearMonth from, int count, int increment = 1)
        {
            var monthCounts = Enumerable.Range(0, count);

            return monthCounts.Select(i => (new YearMonth { MonthCount = from.MonthCount }).AddMonths(i * increment));
        }

        public static IEnumerable<YearMonth> Range(YearMonth from, YearMonth to, int increment = 1)
        {
            if (from.MonthCount > to.MonthCount)
                throw new ArgumentOutOfRangeException($"Invalid YearMonth range, YearFrom:{from.Year} MonthFrom:{Enum.GetName(typeof(MonthValue), from.Month)} YearTo:{to.Year}  MonthTo:{Enum.GetName(typeof(MonthValue), to.Month)}");

            var months = to.MonthCount - from.MonthCount + 1;

            return Range(from, months / increment);
        }

        public int MonthCount
        {
            get
            {
                return (Year * 12) + (int)Month - 1;
            }
            set
            {
                Year = value / 12;
                Month = ((value % 12) + 1).ToMonthValue();
            }
        }

        public YearMonth Clone => new YearMonth { Year = Year, Month = Month };

        public int MonthNumber => (int)Month;

        public int DaysInMonth
        {
            get
            {
                var calendar = CalendarSystem.GetJulianCalendar(1);

                return calendar.GetDaysInMonth(Year, MonthNumber);
            }
        }

        public YearMonth AddYears(int years)
        {
            return AddMonths(years * 12);
        }

        public YearMonth AddMonths(int months)
        {
            return new YearMonth { MonthCount = MonthCount + months };
        } 

        public Date ToLocalDate(
            int day,
            RepeatingDay.RollStrategyType rollStrategyType = RepeatingDay.RollStrategyType.Skip)
        {
            Date localDate;

            if(TryToLocalDate(day, out localDate, rollStrategyType))
            {
                return localDate;
            }

            throw new DateOutOfBoundsException(Year, Month, day);
        }

        public bool TryToLocalDate(
            int day,
            out Date localDate, 
            RepeatingDay.RollStrategyType rollStrategyType = RepeatingDay.RollStrategyType.Skip)
        {
            var year = Year;

            var daysInMonth = DateTimeHelper.GetDaysInMonth(year, Month);

            if (day <= daysInMonth)
            {
                localDate = new Date(Year, Month, day);
                return true;
            }

            switch (rollStrategyType)
            {
                case RepeatingDay.RollStrategyType.Back:
                    localDate = new Date(Year, Month, daysInMonth);
                    return true;

                case RepeatingDay.RollStrategyType.Forward:
                    return AddMonths(1).TryToLocalDate(1, out localDate, RepeatingDay.RollStrategyType.Throw);

                default:
                    localDate = default(Date);
                    return false;
            }
        }
    }
}
