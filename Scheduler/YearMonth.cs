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

        private int year;
        private MonthValue month;

        public int Year
        {
            set
            {
                if (value < 1000 || value > 9999)
                    throw new ArgumentOutOfRangeException(string.Format("Invalid Year value: {0}", value));

                year = value;
            }
            get
            {
                return year;
            }
        }

        public MonthValue Month
        {
            set
            {
                this.month = value;
            }
            get
            {
                return this.month;
            }
        }

        public LocalDate LocalDate
        {
            set
            {
                this.Year = value.Year;
                this.Month = value.Month.ToMonthValue();
            }
        }

        
        public YearMonth()
        {

        }

        public static IEnumerable<YearMonth> Range(YearMonth from, int count, int increment = 1)
        {
            var monthCounts = Enumerable.Range(0, count);

            return monthCounts.Select(i => (new YearMonth() { MonthCount = from.MonthCount }).AddMonths(i * increment));
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

        public YearMonth Clone
        {
            get
            {
                return new YearMonth() { Year = this.Year, Month = this.Month };
            }
        }

        public int MonthNumber
        {
            get { return (int)Month; }
        }

        public int DaysInMonth
        {
            get
            {
                var calendar = NodaTime.CalendarSystem.GetJulianCalendar(1);

                return calendar.GetDaysInMonth(Year, MonthNumber);
            }
        }

        public YearMonth AddYears(int years)
        {
            return AddMonths(years * 12);
        }

        public YearMonth AddMonths(int months)
        {
            return new YearMonth() { MonthCount = this.MonthCount + months };
        } 

        public LocalDate ToLocalDate
            (int day,
            RepeatingDay.RollStrategyType rollStrategyType = RepeatingDay.RollStrategyType.Skip)
        {
            LocalDate? localDate;

            if(!TryToLocalDate(day, out localDate, rollStrategyType))
            {
                throw new DateOutOfBoundsException(this.Year, this.Month, day);
            }

            return localDate.Value;
        }

        public bool TryToLocalDate(
            int day,
            out LocalDate? localDate, 
            RepeatingDay.RollStrategyType rollStrategyType = RepeatingDay.RollStrategyType.Skip)
        {
            var year = Year;
            var month = MonthNumber;

            var daysInMonth = DateTimeHelper.GetDaysInMonth(year, month);

            if (day <= daysInMonth)
            {
                localDate = new LocalDate(Year, month, day);
                return true;
            }

            switch (rollStrategyType)
            {
                case RepeatingDay.RollStrategyType.Back:
                    localDate = new LocalDate(year, month, daysInMonth);
                    return true;

                case RepeatingDay.RollStrategyType.Forward:
                    return AddMonths(1).TryToLocalDate(1, out localDate, RepeatingDay.RollStrategyType.Throw);
            }

            localDate = null;
            return false;
        }

        public LocalDate ToLocalDate(int day = 1)
        {
            return new LocalDate(Year, MonthNumber, day);
        }
    }
}
