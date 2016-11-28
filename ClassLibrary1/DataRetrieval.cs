using NodaTime;
using Scheduler;
using Scheduler.ScheduleInstances;
using System.Collections.Generic;
using System.Linq;

namespace TestData
{
    public class DataRetrieval
    {
        private static Dictionary<string, ISchedule> _scheduleArchive;
        private static Dictionary<string, IEnumerable<IsoDayOfWeek>> _dateTypes;
        private static Dictionary<string, LocalDate> _dates;
        private static Dictionary<string, Range> _ranges;

        public static Dictionary<string, ISchedule> ScheduleArchive
        {
            get
            {
                if (_scheduleArchive != null) return _scheduleArchive;

                _scheduleArchive = new Dictionary<string, ISchedule>
                {
                    {
                        "Schools.Term.201617.Autumn",
                        new ByWeekdays
                        {
                            Range = new Range(
                                Dates["Schools.Term.201617.Autumn.Start"],
                                Dates["Schools.Term.201617.Autumn.End"]),
                            Days = DayRanges["Weekdays"],
                        }
                    },
                    {
                        "Schools.Term.201617.Autumn.1",
                        new ByWeekdays
                        {
                            Range = new Range(
                                Dates["Schools.Term.201617.Autumn.Start"],
                                Dates["Schools.Term.201617.Autumn.HalfTerm.Start"].PlusDays(-1)),
                            Days = DayRanges["Weekdays"],
                        }
                    },
                    {
                        "Schools.Term.201617.Autumn.HalfTerm",
                        new ByWeekdays
                        {
                            Range = new Range(
                                Dates["Schools.Term.201617.Autumn.HalfTerm.Start"],
                                Dates["Schools.Term.201617.Autumn.HalfTerm.End"]),
                            Days = DayRanges["Weekdays"],
                        }
                    },
                    {
                        "Schools.Term.201617.Autumn.2",
                        new ByWeekdays
                        {
                            Range = new Range(
                                Dates["Schools.Term.201617.Autumn.HalfTerm.End"].PlusDays(01),
                                Dates["Schools.Term.201617.Autumn.End"]),
                            Days = DayRanges["Weekdays"],
                        }
                    },
                    {
                        "Schools.Term.201617.Winter",
                        new ByWeekdays
                        {
                            Range = new Range(
                                Dates["Schools.Term.201617.Winter.Start"],
                                Dates["Schools.Term.201617.Winter.End"]),
                            Days = DayRanges["Weekdays"],
                        }
                    },
                    {
                        "Schools.Term.201617.Summer",
                        new ByWeekdays
                        {
                            Range = new Range(
                                Dates["Schools.Term.201617.Summer.Start"],
                                Dates["Schools.Term.201617.Summer.End"]),
                            Days = DayRanges["Weekdays"],
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

                _scheduleArchive
                    .Add("BankHolidays",
                    new DateList
                    {
                        Items = _scheduleArchive
                            .Where(s => s.Key.StartsWith("BankHolidays.2016"))
                            .SelectMany(s => s.Value.Dates)
                    });

                _scheduleArchive
                    .Add("Example.AutumnTerm",
                        new Scheduler.CompositeSchedule()
                        {
                            Inclusions = new List<ISchedule>
                            {
                                TestData.DataRetrieval.ScheduleArchive["Schools.Term.201617.Autumn"],
                                TestData.DataRetrieval.ScheduleArchive["Schools.Term.201617.Winter"],
                                TestData.DataRetrieval.ScheduleArchive["Schools.Term.201617.Summer"],
                            },
                            Exclusions = new List<ISchedule>
                            {
                                TestData.DataRetrieval.ScheduleArchive["BankHolidays"],
                            },
                            Breaks = new List<Range>
                            {
                                TestData.DataRetrieval.Ranges["Schools.Term.201617.Autumn.HalfTerm"],
                                TestData.DataRetrieval.Ranges["Schools.Term.201617.Winter.HalfTerm"],
                                TestData.DataRetrieval.Ranges["Schools.Term.201617.Summer.HalfTerm"],
                            },
                    });

                return _scheduleArchive;
            }
        }

        public static Dictionary<string, Range> Ranges
        {
            get
            {
                if (_ranges != null) return _ranges;

                _ranges = new Dictionary<string, Range>
                {
                    {
                        "Schools.Term.201617.Autumn",
                        new Range(Dates["Schools.Term.201617.Autumn.Start"], Dates["Schools.Term.201617.Autumn.End"])
                    },
                    {
                        "Schools.Term.201617.Winter",
                        new Range(Dates["Schools.Term.201617.Winter.Start"], Dates["Schools.Term.201617.Winter.End"])
                    },
                    {
                        "Schools.Term.201617.Summer",
                        new Range(Dates["Schools.Term.201617.Summer.Start"], Dates["Schools.Term.201617.Summer.End"])
                    },
                    {
                        "Schools.Term.201718.Autumn",
                        new Range(Dates["Schools.Term.201718.Autumn.Start"], Dates["Schools.Term.201718.Autumn.End"])
                    },
                    {
                        "Schools.Term.201617.Autumn.HalfTerm",
                        new Range(Dates["Schools.Term.201617.Autumn.HalfTerm.Start"], Dates["Schools.Term.201617.Autumn.HalfTerm.End"])
                    },
                    {
                        "Schools.Term.201617.Winter.HalfTerm",
                        new Range(Dates["Schools.Term.201617.Winter.HalfTerm.Start"], Dates["Schools.Term.201617.Winter.HalfTerm.End"])
                    },
                    {
                        "Schools.Term.201617.Summer.HalfTerm",
                        new Range(Dates["Schools.Term.201617.Summer.HalfTerm.Start"], Dates["Schools.Term.201617.Summer.HalfTerm.End"])
                    },
                };

                return _ranges;
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
                    "Schools.Term.201617.Autumn.Start",
                    DateTimeHelper.GetLocalDate(2016, YearMonth.MonthValue.August, 05)
                },
                {
                    "Schools.Term.201617.Autumn.HalfTerm.Start",
                    DateTimeHelper.GetLocalDate(2016, YearMonth.MonthValue.October, 24)
                },
                {
                    "Schools.Term.201617.Autumn.HalfTerm.End",
                    DateTimeHelper.GetLocalDate(2016, YearMonth.MonthValue.October, 31)
                },
                {
                    "Schools.Term.201617.Autumn.End",
                    DateTimeHelper.GetLocalDate(2016, YearMonth.MonthValue.December, 21)
                },
                {
                    "Schools.Term.201617.Winter.Start",
                    DateTimeHelper.GetLocalDate(2017, YearMonth.MonthValue.January, 05)
                },
                {
                    "Schools.Term.201617.Winter.HalfTerm.Start",
                    DateTimeHelper.GetLocalDate(2017, YearMonth.MonthValue.February, 24)
                },
                {
                    "Schools.Term.201617.Winter.HalfTerm.End",
                    DateTimeHelper.GetLocalDate(2017, YearMonth.MonthValue.March, 01)
                },
                {
                    "Schools.Term.201617.Winter.End",
                    DateTimeHelper.GetLocalDate(2017, YearMonth.MonthValue.April, 10)
                },
                {
                    "Schools.Term.201617.Summer.Start",
                    DateTimeHelper.GetLocalDate(2017, YearMonth.MonthValue.April, 30)
                },
                {
                    "Schools.Term.201617.Summer.HalfTerm.Start",
                    DateTimeHelper.GetLocalDate(2017, YearMonth.MonthValue.May, 26)
                },
                {
                    "Schools.Term.201617.Summer.HalfTerm.End",
                    DateTimeHelper.GetLocalDate(2017, YearMonth.MonthValue.June, 03)
                },
                {
                    "Schools.Term.201617.Summer.End",
                    DateTimeHelper.GetLocalDate(2017, YearMonth.MonthValue.July, 17)
                },
                {
                    "Schools.Term.201718.Autumn.Start",
                    DateTimeHelper.GetLocalDate(2017, YearMonth.MonthValue.August, 05)
                },
                {
                    "Schools.Term.201718.Autumn.HalfTerm.Start",
                    DateTimeHelper.GetLocalDate(2017, YearMonth.MonthValue.October, 24)
                },
                {
                    "Schools.Term.201718.Autumn.HalfTerm.End",
                    DateTimeHelper.GetLocalDate(2017, YearMonth.MonthValue.November, 01)
                },
                {
                    "Schools.Term.201718.Autumn.End",
                    DateTimeHelper.GetLocalDate(2017, YearMonth.MonthValue.December, 21)
                },
            });
    }
}
