using NodaTime;
using Scheduler;
using Scheduler.ScheduleInstances;
using System.Collections.Generic;
using System.Linq;
using Scheduler.Persistance;
using Scheduler.Ranges;
using Scheduler.ScheduleEdges;
using Scheduler.Users;

namespace TestData
{
    public class DataRetrieval
    {
        private static Dictionary<string, ISchedule> _scheduleArchive;
        private static Dictionary<string, IEnumerable<IsoDayOfWeek>> _dateTypes;
        private static Dictionary<string, IDate> _dates;
        private static Dictionary<string, RangeDate> _ranges;
        private static Dictionary<string, Organisation> _organisations;

        public static Dictionary<string, ISchedule> GetScheduleArchive(IClock clock)
        {
            if (_scheduleArchive != null) return _scheduleArchive;

            _scheduleArchive = new Dictionary<string, ISchedule>
            {
                {
                    "Schools.Term.201617.Autumn",
                    new ByWeekdays
                    {
                        EdgeRange = new EdgeRangeDate(
                            start: Dates["Schools.Term.201617.Autumn.Start"],
                            end: Dates["Schools.Term.201617.Autumn.End"]),
                        Days = DayRanges["Weekdays"],
                    }
                },
                {
                    "Schools.Term.201617.Autumn.1",
                    new ByWeekdays
                    {
                        EdgeRange = new EdgeRangeDate(
                            start: Dates["Schools.Term.201617.Autumn.Start"],
                            end: Dates["Schools.Term.201617.Autumn.HalfTerm.Start"].PlusDays(-1)),
                        Days = DayRanges["Weekdays"],
                    }
                },
                {
                    "Schools.Term.201617.Autumn.HalfTerm",
                    new ByWeekdays
                    {
                        EdgeRange = new EdgeRangeDate(
                            start: Dates["Schools.Term.201617.Autumn.HalfTerm.Start"],
                            end: Dates["Schools.Term.201617.Autumn.HalfTerm.End"]),
                        Days = DayRanges["Weekdays"],
                    }
                },
                {
                    "Schools.Term.201617.Autumn.2",
                    new ByWeekdays
                    {
                        EdgeRange = new EdgeRangeDate(
                            start: Dates["Schools.Term.201617.Autumn.HalfTerm.End"].PlusDays(01),
                            end: Dates["Schools.Term.201617.Autumn.End"]),
                        Days = DayRanges["Weekdays"],
                    }
                },
                {
                    "Schools.Term.201617.Winter",
                    new ByWeekdays
                    {
                        EdgeRange = new EdgeRangeDate(
                            start: Dates["Schools.Term.201617.Winter.Start"],
                            end: Dates["Schools.Term.201617.Winter.End"]),
                        Days = DayRanges["Weekdays"],
                    }
                },
                {
                    "Schools.Term.201617.Summer",
                    new ByWeekdays
                    {
                        EdgeRange = new EdgeRangeDate(
                            start: Dates["Schools.Term.201617.Summer.Start"],
                            end: Dates["Schools.Term.201617.Summer.End"]),
                        Days = DayRanges["Weekdays"],
                    }
                },
                {
                    "BankHolidays.2016.NewYearsDay",
                    new SingleDay {Date = new Date(2016, YearMonth.MonthValue.January, 01)}
                },
                {
                    "BankHolidays.2016.GoodFriday",
                    new SingleDay {Date = new Date(2016, YearMonth.MonthValue.March, 25)}
                },
                {
                    "BankHolidays.2016.EasterMonday",
                    new SingleDay {Date = new Date(2016, YearMonth.MonthValue.March, 28)}
                },
                {
                    "BankHolidays.2016.EarlyMayBankHoliday",
                    new SingleDay {Date = new Date(2016, YearMonth.MonthValue.May, 01)}
                },
                {
                    "BankHolidays.2016.SpringBankHoliday",
                    new SingleDay {Date = new Date(2016, YearMonth.MonthValue.May, 30)}
                },
                {
                    "BankHolidays.2016.SummerBankHoliday",
                    new SingleDay {Date = new Date(2016, YearMonth.MonthValue.August, 29)}
                },
                {
                    "BankHolidays.2016.Boxing",
                    new SingleDay {Date = new Date(2016, YearMonth.MonthValue.December, 26)}
                },
                {
                    "BankHolidays.2016.ChristmasDaySubstitute",
                    new SingleDay {Date = new Date(2016, YearMonth.MonthValue.December, 27)}
                }
            };

            _scheduleArchive
                .Add("BankHolidays",
                    ByDateList.Create(
                        _scheduleArchive
                            .Where(s => s.Key.StartsWith("BankHolidays.2016"))
                            .SelectMany(s => s.Value.Generate(clock)
                            .Select(date => date))
                    ));

            _scheduleArchive
                .Add("Example.AutumnTerm",
                    new CompositeSchedule()
                    {
                        Inclusions = new EdgeVertexs<ISchedule>
                        {
                            new EdgeVertex<ISchedule>(_scheduleArchive["Schools.Term.201617.Autumn"]),
                            new EdgeVertex<ISchedule>(_scheduleArchive["Schools.Term.201617.Winter"]),
                            new EdgeVertex<ISchedule>(_scheduleArchive["Schools.Term.201617.Summer"]),
                        },
                        Exclusions = new EdgeVertexs<ISchedule>
                        {
                            new EdgeVertex<ISchedule>(_scheduleArchive["BankHolidays"]),
                        },
                        Breaks = new EdgeVertexs<IRangeDate>()
                        {
                            new EdgeVertex<IRangeDate>(Ranges["Schools.Term.201617.Autumn.HalfTerm"]),
                            new EdgeVertex<IRangeDate>(Ranges["Schools.Term.201617.Winter.HalfTerm"]),
                            new EdgeVertex<IRangeDate>(Ranges["Schools.Term.201617.Summer.HalfTerm"]),
                        },
                    });

            return _scheduleArchive;
        }

        public static Dictionary<string, Organisation> Organisations
        {
            get
            {
                if (_organisations != null) return _organisations;

                _organisations = new Dictionary<string, Organisation>
                {
                    {
                        "Hampden Gurney Primary School",
                        new Organisation {Title = "Hampden Gurney Primary School", }
                    },
                    {
                        "Sylvia Young Theatre School",
                        new Organisation {Title = "Sylvia Young Theatre School", }
                    },
                    {
                        "Lords Cricket Academy",
                        new Organisation
                        {
                            Title = "Lords Cricket Academy",
                            Location = new EdgeVertex<Location>(new Location
                            {
                                Address = "St Johns Wood",
                            })
                        }
                    },
                };

                return _organisations;
            }
        }

        public static Dictionary<string, RangeDate> Ranges
        {
            get
            {
                if (_ranges != null) return _ranges;

                _ranges = new Dictionary<string, RangeDate>
                {
                    {
                        "Schools.Term.201617.Autumn",
                        new RangeDateBuilder
                        {
                            Start = Dates["Schools.Term.201617.Autumn.Start"],
                            End = Dates["Schools.Term.201617.Autumn.End"]
                        }.Build()
                    },
                    {
                        "Schools.Term.201617.Winter",
                        new RangeDateBuilder
                        {
                            Start = Dates["Schools.Term.201617.Winter.Start"],
                            End = Dates["Schools.Term.201617.Winter.End"]
                        }.Build()
                    },
                    {
                        "Schools.Term.201617.Summer",
                        new RangeDateBuilder
                        {
                            Start = Dates["Schools.Term.201617.Summer.Start"],
                            End = Dates["Schools.Term.201617.Summer.End"]
                        }.Build()
                    },
                    {
                        "Schools.Term.201718.Autumn",
                        new RangeDateBuilder
                        {
                            Start = Dates["Schools.Term.201718.Autumn.Start"],
                            End = Dates["Schools.Term.201718.Autumn.End"]
                        }.Build()
                    },
                    {
                        "Schools.Term.201617.Autumn.HalfTerm",
                        new RangeDateBuilder
                        {
                            Start = Dates["Schools.Term.201617.Autumn.HalfTerm.Start"],
                            End = Dates["Schools.Term.201617.Autumn.HalfTerm.End"]
                        }.Build()
                    },
                    {
                        "Schools.Term.201617.Winter.HalfTerm",
                        new RangeDateBuilder
                        {
                            Start = Dates["Schools.Term.201617.Winter.HalfTerm.Start"],
                            End = Dates["Schools.Term.201617.Winter.HalfTerm.End"]
                        }.Build()
                    },
                    {
                        "Schools.Term.201617.Summer.HalfTerm",
                        new RangeDateBuilder
                        {
                            Start = Dates["Schools.Term.201617.Summer.HalfTerm.Start"],
                            End = Dates["Schools.Term.201617.Summer.HalfTerm.End"]
                        }.Build()
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

        public static Dictionary<string, IDate> Dates
            => _dates ?? (_dates = new Dictionary<string, IDate>
               {
                    {
                        "Schools.Term.201617.Autumn.Start",
                        new Date(2016, YearMonth.MonthValue.August, 05)
                    },
                    {
                        "Schools.Term.201617.Autumn.HalfTerm.Start",
                        new Date(2016, YearMonth.MonthValue.October, 24)
                    },
                    {
                        "Schools.Term.201617.Autumn.HalfTerm.End",
                        new Date(2016, YearMonth.MonthValue.October, 31)
                    },
                    {
                        "Schools.Term.201617.Autumn.End",
                        new Date(2016, YearMonth.MonthValue.December, 21)
                    },
                    {
                        "Schools.Term.201617.Winter.Start",
                        new Date(2017, YearMonth.MonthValue.January, 05)
                    },
                    {
                        "Schools.Term.201617.Winter.HalfTerm.Start",
                        new Date(2017, YearMonth.MonthValue.February, 24)
                    },
                    {
                        "Schools.Term.201617.Winter.HalfTerm.End",
                        new Date(2017, YearMonth.MonthValue.March, 01)
                    },
                    {
                        "Schools.Term.201617.Winter.End",
                        new Date(2017, YearMonth.MonthValue.April, 10)
                    },
                    {
                        "Schools.Term.201617.Summer.Start",
                        new Date(2017, YearMonth.MonthValue.April, 30)
                    },
                    {
                        "Schools.Term.201617.Summer.HalfTerm.Start",
                        new Date(2017, YearMonth.MonthValue.May, 26)
                    },
                    {
                        "Schools.Term.201617.Summer.HalfTerm.End",
                        new Date(2017, YearMonth.MonthValue.June, 03)
                    },
                    {
                        "Schools.Term.201617.Summer.End",
                        new Date(2017, YearMonth.MonthValue.July, 17)
                    },
                    {
                        "Schools.Term.201718.Autumn.Start",
                        new Date(2017, YearMonth.MonthValue.August, 05)
                    },
                    {
                        "Schools.Term.201718.Autumn.HalfTerm.Start",
                        new Date(2017, YearMonth.MonthValue.October, 24)
                    },
                    {
                        "Schools.Term.201718.Autumn.HalfTerm.End",
                        new Date(2017, YearMonth.MonthValue.November, 01)
                    },
                    {
                        "Schools.Term.201718.Autumn.End",
                        new Date(2017, YearMonth.MonthValue.December, 21)
                    },
            }
            );
    }
}
