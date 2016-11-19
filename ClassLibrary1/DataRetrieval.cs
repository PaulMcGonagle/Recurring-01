using NodaTime;
using Scheduler;
using Scheduler.ScheduleInstances;
using System.Collections.Generic;

namespace TestData
{
    public class DataRetrieval
    {
        private static Dictionary<string, ISchedule> _scheduleArchive;
        private static Dictionary<string, IEnumerable<IsoDayOfWeek>> _dateTypes;
        private static Dictionary<string, LocalDate> _dates;

        public static Dictionary<string, ISchedule> ScheduleArchive
        {
            get
            {
                if (_scheduleArchive != null) return _scheduleArchive;

                _scheduleArchive = new Dictionary<string, ISchedule>
                {
                    {
                        "Schools.Term.Autumn",
                        new ByWeekdays
                        {
                            DateFrom = Dates["Schools.Term.Autumn.Start"],
                            DateTo = Dates["Schools.Term.Autumn.Start"],
                            Days = DayRanges["Weekends"],
                        }
                    },
                    {
                        "Schools.Term.Autumn",
                        new ByWeekdays
                        {
                            DateFrom = Dates["Schools.Term.Autumn.Start"],
                            DateTo = Dates["Schools.Term.Autumn.Start"],
                            Days = DayRanges["Weekends"],
                        }
                    },
                    {
                        "Schools.Term.Autumn.1",
                        new ByWeekdays
                        {
                            DateFrom = Dates["Schools.Term.Autumn.Start"],
                            DateTo = Dates["Schools.Term.Autumn.HalfTerm.Start"].PlusDays(-1),
                            Days = DayRanges["Weekends"],
                        }
                    },
                    {
                        "Schools.Term.Autumn.HalfTerm",
                        new ByWeekdays
                        {
                            DateFrom = Dates["Schools.Term.Autumn.HalfTerm.Start"],
                            DateTo = Dates["Schools.Term.Autumn.HalfTerm.End"],
                            Days = DayRanges["Weekends"],
                        }
                    },
                    {
                        "Schools.Term.Autumn.2",
                        new ByWeekdays
                        {
                            DateFrom = Dates["Schools.Term.Autumn.HalfTerm.End"].PlusDays(01),
                            DateTo = Dates["Schools.Term.Autumn.End"],
                            Days = DayRanges["Weekends"],
                        }
                    },
                    {
                        "BankHolidays.2016.NewYearsDay",
                        new SingleDay {Date = DateTimeHelper.GetLocalDate(2016, YearMonth.MonthValue.January, 01)}
                    },
                    {
                        "BankHolidays.2016.GoodFriday",
                        new SingleDay {Date = DateTimeHelper.GetLocalDate(2016, YearMonth.MonthValue.March, 25)}
                    },
                    {
                        "BankHolidays.2016.EasterMonday",
                        new SingleDay {Date = DateTimeHelper.GetLocalDate(2016, YearMonth.MonthValue.March, 28)}
                    },
                    {
                        "BankHolidays.2016.EarlyMayBankHoliday",
                        new SingleDay {Date = DateTimeHelper.GetLocalDate(2016, YearMonth.MonthValue.May, 01)}
                    },
                    {
                        "BankHolidays.2016.SpringBankHoliday",
                        new SingleDay {Date = DateTimeHelper.GetLocalDate(2016, YearMonth.MonthValue.May, 30)}
                    },
                    {
                        "BankHolidays.2016.SummerBankHoliday",
                        new SingleDay {Date = DateTimeHelper.GetLocalDate(2016, YearMonth.MonthValue.August, 29)}
                    },
                    {
                        "BankHolidays.2016.Boxing",
                        new SingleDay {Date = DateTimeHelper.GetLocalDate(2016, YearMonth.MonthValue.December, 26)}
                    },
                    {
                        "BankHolidays.2016.ChristmasDaySubstitute",
                        new SingleDay {Date = DateTimeHelper.GetLocalDate(2016, YearMonth.MonthValue.December, 27)}
                    }
                };

                return _scheduleArchive;
            }
        }

        public static Dictionary<string, IEnumerable<IsoDayOfWeek>> DayRanges
        {
            get
            {
                if (_dateTypes != null) return _dateTypes;

                _dateTypes = new Dictionary<string, IEnumerable<IsoDayOfWeek>>
                {
                    {
                        "Weekdays",
                        new List<IsoDayOfWeek>
                        {
                            IsoDayOfWeek.Monday,
                            IsoDayOfWeek.Tuesday,
                            IsoDayOfWeek.Wednesday,
                            IsoDayOfWeek.Thursday,
                            IsoDayOfWeek.Friday
                        }
                    },
                    {
                        "Weekends",
                        new List<IsoDayOfWeek>
                        {
                            IsoDayOfWeek.Saturday,
                            IsoDayOfWeek.Sunday
                        }
                    },
                };
                return _dateTypes;
            }
        }

        public static Dictionary<string, LocalDate> Dates => _dates ?? (_dates = new Dictionary<string, LocalDate>
            {
                {
                    "Schools.Term.Autumn.Start",
                    DateTimeHelper.GetLocalDate(2016, YearMonth.MonthValue.October, 05)
                },
                {
                    "Schools.Term.Autumn.HalfTerm.Start",
                    DateTimeHelper.GetLocalDate(2016, YearMonth.MonthValue.October, 24)
                },
                {
                    "Schools.Term.Autumn.HalfTerm.End",
                    DateTimeHelper.GetLocalDate(2016, YearMonth.MonthValue.October, 31)
                },
                {
                    "Schools.Term.Autumn.End",
                    DateTimeHelper.GetLocalDate(2016, YearMonth.MonthValue.December, 21)
                },
            });
    }
}
