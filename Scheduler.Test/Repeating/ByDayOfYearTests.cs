﻿using NodaTime;
using Scheduler.ScheduleAbstracts;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using TestStack.BDDfy;
using Scheduler.ScheduleInstances;
using Xunit;

namespace Scheduler.Test.Repeating
{
    public class ByDayOfYearTests
    {
        public class VerifyDateOutOfBoundsExceptionIsThrown
        {
            private ByDayOfYear _sut;
            private System.Exception _exception;

            [Fact]
            public void Execute()
            {
                const int defaultYear = 2015;
                const int leapYear = 2000;
                const int defaultMonth = 05;
                const int defaultDay = 01;

                var fakeClockNonLeap = ScheduleTestHelper.GetFakeClock(defaultYear, defaultMonth, defaultDay);
                var fakeClockLeapYear = ScheduleTestHelper.GetFakeClock(leapYear, defaultMonth, defaultDay);

                this.WithExamples(new ExampleTable("sut")
                    {
                        {
                            new ByDayOfYear
                            {
                                DayOfYear = 29,
                                Month = YearMonth.MonthValue.February,
                                Clock = fakeClockNonLeap,
                                CountFrom = -02,
                                CountTo = 02,
                                RollStrategy = RepeatingDay.RollStrategyType.Throw
                            }
                        },
                        {
                            new ByDayOfYear
                            {
                                DayOfYear = 30,
                                Month = YearMonth.MonthValue.February,
                                Clock = fakeClockLeapYear,
                                CountFrom = -02,
                                CountTo = 02,
                                RollStrategy = RepeatingDay.RollStrategyType.Throw
                            }
                        },
                        {
                            new ByDayOfYear
                            {
                                DayOfYear = 31,
                                Month = YearMonth.MonthValue.February,
                                Clock = fakeClockLeapYear,
                                CountFrom = -02,
                                CountTo = 02,
                                RollStrategy = RepeatingDay.RollStrategyType.Throw
                            }
                        },
                        {
                            new ByDayOfYear
                            {
                                DayOfYear = 31,
                                Month = YearMonth.MonthValue.April,
                                Clock = fakeClockLeapYear,
                                CountFrom = -02,
                                CountTo = 02,
                                RollStrategy = RepeatingDay.RollStrategyType.Throw
                            }
                        },
                    })
                    .BDDfy();
            }

            public void GivenARepeatingDay(ByDayOfYear sut)
            {
                _sut = sut;
            }

            public void WhenDatesAreReturned()
            {
                _exception = Record.Exception(() => { var dates = _sut.Dates.ToList(); });
            }

            public void ThenDatesThrowsADateOutOfBoundsException(int expectedExceptionYear,
                YearMonth.MonthValue expectedExceptionMonth, int expectedExceptionDay)
            {
                _exception.ShouldNotBeNull();
                _exception.ShouldBeOfType<DateOutOfBoundsException>();
            }
        }

        public class InvalidDayShouldThrowException
        {
            private YearMonth.MonthValue _month;
            private int _day;

            private IClock _clock;
            private System.Exception _exception;

            [Fact]
            public void Execute()
            {
                const int defaultYear = 2015;
                const int leapYear = 2000;
                const int defaultMonth = 05;
                const int defaultDay = 01;

                var fakeClockLeapPre = ScheduleTestHelper.GetFakeClock(defaultYear, defaultMonth, defaultDay);
                var fakeClockLeapYear = ScheduleTestHelper.GetFakeClock(leapYear, defaultMonth, defaultDay);

                this.WithExamples(new ExampleTable("Month", "Day", "Clock", "Is exception expected")
                    {
                        {YearMonth.MonthValue.January, -1, fakeClockLeapPre, true},
                        {YearMonth.MonthValue.January, 01, fakeClockLeapPre, false},
                        {YearMonth.MonthValue.January, 31, fakeClockLeapPre, false},
                        {YearMonth.MonthValue.January, 32, fakeClockLeapYear, true},
                        {YearMonth.MonthValue.February, 29, fakeClockLeapPre, false},
                    })
                    .BDDfy();
            }

            public void GivenAMonth(YearMonth.MonthValue month)
            {
                _month = month;
            }

            public void AndGivenADay(int day)
            {
                _day = day;
            }

            public void AndGivenAClock(IClock clock)
            {
                _clock = clock;
            }

            public void WhenSutIsInstantiated()
            {
                _exception = Record.Exception(() => { var byDayOfYear = new ByDayOfYear {Month = _month, DayOfYear = _day, Clock = _clock}; });
            }

            public void ThenAnExceptionIsThrown(bool isExceptionExpected)
            {
                if (isExceptionExpected)
                    _exception.ShouldNotBeNull();
                else
                    _exception.ShouldBeNull();
            }
        }

        public class ValidateDates
        {
            private ByDayOfYear _sut;
            private IEnumerable<LocalDate> _dates;

            [Fact]
            public void Execute()
            {
                const int yearLeap = 2106;
                const YearMonth.MonthValue monthClock = YearMonth.MonthValue.May;
                const int dayClock = 01;

                var fakeClock = ScheduleTestHelper.GetFakeClock(yearLeap, monthClock, dayClock);

                this.WithExamples(new ExampleTable("SUT", "Expected Dates")
                    {
                        {
                            new ByDayOfYear
                            {
                                DayOfYear = 01,
                                Month = YearMonth.MonthValue.January,
                                Clock = fakeClock,
                                CountFrom = -02,
                                CountTo = 12
                            },
                            Enumerable.Range(yearLeap - 02, 15)
                                .Select(year => DateTimeHelper.GetLocalDate(year, YearMonth.MonthValue.January, 01))
                        },
                        {
                            new ByDayOfYear
                            {
                                DayOfYear = 28,
                                Month = YearMonth.MonthValue.January,
                                Clock = fakeClock,
                                CountFrom = -02,
                                CountTo = 12
                            },
                            Enumerable.Range(yearLeap - 02, 15)
                                .Select(year => DateTimeHelper.GetLocalDate(year, YearMonth.MonthValue.January, 28))
                        },
                        {
                            new ByDayOfYear
                            {
                                DayOfYear = 15,
                                Month = YearMonth.MonthValue.January,
                                Clock = fakeClock,
                                CountFrom = 02,
                                CountTo = 20
                            },
                            Enumerable.Range(yearLeap + 02, 19)
                                .Select(year => DateTimeHelper.GetLocalDate(year, YearMonth.MonthValue.January, 15))
                        },
                        {
                            new ByDayOfYear
                            {
                                DayOfYear = 05,
                                Month = YearMonth.MonthValue.April,
                                Clock = fakeClock,
                                DateFrom = DateTimeHelper.GetLocalDate(2016, YearMonth.MonthValue.March, 01),
                                DateTo = DateTimeHelper.GetLocalDate(2020, YearMonth.MonthValue.April, 30),
                                RollStrategy = RepeatingDay.RollStrategyType.Skip
                            },
                            Enumerable.Range(2016, 5)
                                .Select(year => DateTimeHelper.GetLocalDate(year, YearMonth.MonthValue.April, 05))
                        },

                    })
                    .BDDfy();
            }

            public void GivenAByDayOfYear(ByDayOfYear sut)
            {
                _sut = sut;
            }

            public void WhenDatesAreRetrieved()
            {
                _dates = _sut.Dates;
            }

            public void ThenAllDatesShouldBeThese(IEnumerable<LocalDate> expectedDates)
            {
                _dates.ShouldBe(expectedDates);
            }
        }

        public class ValidateRollStrategy
        {
            private ByDayOfYear _sut;
            private IEnumerable<LocalDate> _dates;

            [Fact]
            public void Execute()
            {
                const int yearLeap = 2106;
                const YearMonth.MonthValue baseMonth = YearMonth.MonthValue.May;
                const int baseDay = 01;

                var fakeClock = ScheduleTestHelper.GetFakeClock(yearLeap, baseMonth, baseDay);

                this.WithExamples(new ExampleTable("SUT", "Expected Dates")
                    {
                        {
                            new ByDayOfYear
                            {
                                DayOfYear = 30,
                                Month = YearMonth.MonthValue.February,
                                Clock = fakeClock,
                                CountFrom = -02,
                                CountTo = 12,
                                RollStrategy = RepeatingDay.RollStrategyType.Forward
                            },
                            Enumerable.Range(yearLeap - 02, 15)
                                .Select(year => DateTimeHelper.GetLocalDate(year, YearMonth.MonthValue.March, 01))
                        },

                        {
                            new ByDayOfYear
                            {
                                DayOfYear = 31,
                                Month = YearMonth.MonthValue.April,
                                Clock = fakeClock,
                                CountFrom = -02,
                                CountTo = 12,
                                RollStrategy = RepeatingDay.RollStrategyType.Forward
                            },
                            Enumerable.Range(yearLeap - 02, 15)
                                .Select(year => DateTimeHelper.GetLocalDate(year, YearMonth.MonthValue.May, 01))
                        },

                        {
                            new ByDayOfYear
                            {
                                DayOfYear = 29,
                                Month = YearMonth.MonthValue.February,
                                Clock = fakeClock,
                                CountFrom = -02,
                                CountTo = 02,
                                RollStrategy = RepeatingDay.RollStrategyType.Forward
                            },
                            new List<LocalDate>
                            {
                                DateTimeHelper.GetLocalDate(yearLeap - 02, YearMonth.MonthValue.February, 29),
                                DateTimeHelper.GetLocalDate(yearLeap - 01, YearMonth.MonthValue.March, 01),
                                DateTimeHelper.GetLocalDate(yearLeap + 00, YearMonth.MonthValue.March, 01),
                                DateTimeHelper.GetLocalDate(yearLeap + 01, YearMonth.MonthValue.March, 01),
                                DateTimeHelper.GetLocalDate(yearLeap + 02, YearMonth.MonthValue.February, 29),
                            }
                        },

                        {
                            new ByDayOfYear
                            {
                                DayOfYear = 30,
                                Month = YearMonth.MonthValue.February,
                                Clock = fakeClock,
                                CountFrom = -02,
                                CountTo = 12,
                                RollStrategy = RepeatingDay.RollStrategyType.Forward
                            },
                            Enumerable.Range(yearLeap - 02, 15)
                                .Select(year => DateTimeHelper.GetLocalDate(year, YearMonth.MonthValue.March, 01))
                        },

                        {
                            new ByDayOfYear
                            {
                                DayOfYear = 29,
                                Month = YearMonth.MonthValue.February,
                                Clock = fakeClock,
                                CountFrom = -02,
                                CountTo = 02,
                                RollStrategy = RepeatingDay.RollStrategyType.Back
                            },
                            new List<LocalDate>
                            {
                                DateTimeHelper.GetLocalDate(yearLeap - 02, YearMonth.MonthValue.February, 29),
                                DateTimeHelper.GetLocalDate(yearLeap - 01, YearMonth.MonthValue.February, 28),
                                DateTimeHelper.GetLocalDate(yearLeap + 00, YearMonth.MonthValue.February, 28),
                                DateTimeHelper.GetLocalDate(yearLeap + 01, YearMonth.MonthValue.February, 28),
                                DateTimeHelper.GetLocalDate(yearLeap + 02, YearMonth.MonthValue.February, 29),
                            }
                        },

                        {
                            new ByDayOfYear
                            {
                                DayOfYear = 30,
                                Month = YearMonth.MonthValue.February,
                                Clock = fakeClock,
                                CountFrom = -02,
                                CountTo = 02,
                                RollStrategy = RepeatingDay.RollStrategyType.Back
                            },
                            new List<LocalDate>
                            {
                                DateTimeHelper.GetLocalDate(yearLeap - 02, YearMonth.MonthValue.February, 29),
                                DateTimeHelper.GetLocalDate(yearLeap - 01, YearMonth.MonthValue.February, 28),
                                DateTimeHelper.GetLocalDate(yearLeap + 00, YearMonth.MonthValue.February, 28),
                                DateTimeHelper.GetLocalDate(yearLeap + 01, YearMonth.MonthValue.February, 28),
                                DateTimeHelper.GetLocalDate(yearLeap + 02, YearMonth.MonthValue.February, 29),
                            }
                        },

                        {
                            new ByDayOfYear
                            {
                                DayOfYear = 31,
                                Month = YearMonth.MonthValue.April,
                                Clock = fakeClock,
                                CountFrom = -02,
                                CountTo = 02,
                                RollStrategy = RepeatingDay.RollStrategyType.Back
                            },
                            new List<LocalDate>
                            {
                                DateTimeHelper.GetLocalDate(yearLeap - 02, YearMonth.MonthValue.April, 30),
                                DateTimeHelper.GetLocalDate(yearLeap - 01, YearMonth.MonthValue.April, 30),
                                DateTimeHelper.GetLocalDate(yearLeap + 00, YearMonth.MonthValue.April, 30),
                                DateTimeHelper.GetLocalDate(yearLeap + 01, YearMonth.MonthValue.April, 30),
                                DateTimeHelper.GetLocalDate(yearLeap + 02, YearMonth.MonthValue.April, 30),
                            }
                        },
                    })
                    .BDDfy();
            }

            public void GivenAByDayOfYear(ByDayOfYear sut)
            {
                _sut = sut;
            }

            public void WhenDatesAreRetrieved()
            {
                _dates = _sut.Dates;
            }

            public void ThenAllDatesShouldBeThese(IEnumerable<LocalDate> expectedDates)
            {
                _dates.ShouldBe(expectedDates);
            }
        }
    }
}