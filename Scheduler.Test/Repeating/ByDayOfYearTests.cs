using NodaTime;
using Scheduler.ScheduleAbstracts;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using Scheduler.ScheduleEdges;
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
            private IClock _clock;
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

                this.WithExamples(new ExampleTable("sut", "clock")
                    {
                        {
                            new ByDayOfYear.Builder
                            {
                                DayOfYear = 29,
                                Month = YearMonth.MonthValue.February,
                                CountFrom = -02,
                                CountTo = 02,
                                RollStrategy = RepeatingDay.RollStrategyType.Throw
                            }.Build(),
                            fakeClockLeapYear
                        },
                        {
                            new ByDayOfYear.Builder
                            {
                                DayOfYear = 30,
                                Month = YearMonth.MonthValue.February,
                                CountFrom = -02,
                                CountTo = 02,
                                RollStrategy = RepeatingDay.RollStrategyType.Throw
                            }.Build(),
                            fakeClockLeapYear
                        },
                        {
                            new ByDayOfYear.Builder
                            {
                                DayOfYear = 31,
                                Month = YearMonth.MonthValue.February,
                                CountFrom = -02,
                                CountTo = 02,
                                RollStrategy = RepeatingDay.RollStrategyType.Throw
                            }.Build(),
                            fakeClockLeapYear
                        },
                        {
                            new ByDayOfYear.Builder
                            {
                                DayOfYear = 31,
                                Month = YearMonth.MonthValue.April,
                                CountFrom = -02,
                                CountTo = 02,
                                RollStrategy = RepeatingDay.RollStrategyType.Throw
                            }.Build(),
                            fakeClockLeapYear
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
                _exception = Record.Exception(() => { var dates = _sut.Generate(_clock).ToList(); });
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
                _exception = Record.Exception(() => { var byDayOfYear = new ByDayOfYear.Builder {Month = _month, DayOfYear = _day}.Build(); });
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
            private IClock _clock;
            private IEnumerable<IDate> _dates;

            [Fact]
            public void Execute()
            {
                const int yearLeap = 2106;
                const YearMonth.MonthValue monthClock = YearMonth.MonthValue.May;
                const int dayClock = 01;

                var fakeClock = ScheduleTestHelper.GetFakeClock(yearLeap, monthClock, dayClock);

                this.WithExamples(new ExampleTable("sut", "clock", "Expected Dates")
                    {
                        {
                            new ByDayOfYear.Builder
                            {
                                DayOfYear = 01,
                                Month = YearMonth.MonthValue.January,
                                CountFrom = -02,
                                CountTo = 12
                            }.Build(),
                            fakeClock,
                            Enumerable.Range(yearLeap - 02, 15)
                                .Select(year => new Date(year, YearMonth.MonthValue.January, 01))
                        },
                        {
                            new ByDayOfYear.Builder
                            {
                                DayOfYear = 28,
                                Month = YearMonth.MonthValue.January,
                                CountFrom = -02,
                                CountTo = 12
                            }.Build(),
                            fakeClock,
                            Enumerable.Range(yearLeap - 02, 15)
                                .Select(year => new Date(year, YearMonth.MonthValue.January, 28))
                        },
                        {
                            new ByDayOfYear.Builder
                            {
                                DayOfYear = 15,
                                Month = YearMonth.MonthValue.January,
                                CountFrom = 02,
                                CountTo = 20
                            }.Build(),
                            fakeClock,
                            Enumerable.Range(yearLeap + 02, 19)
                                .Select(year => new Date(year, YearMonth.MonthValue.January, 15))
                        },
                        {
                            new ByDayOfYear.Builder
                            {
                                DayOfYear = 05,
                                Month = YearMonth.MonthValue.April,
                                EdgeRangeDate = new EdgeRangeDate(new Date(2016, YearMonth.MonthValue.March, 01), new Date(2020, YearMonth.MonthValue.April, 30)),
                                RollStrategy = RepeatingDay.RollStrategyType.Skip
                            }.Build(),
                            fakeClock,
                            Enumerable.Range(2016, 5)
                                .Select(year => new Date(year, YearMonth.MonthValue.April, 05))
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
                _dates = _sut.Generate(_clock);
            }

            public void ThenAllDatesShouldBeThese(IEnumerable<IDate> expectedDates)
            {
                _dates
                    .ShouldBe(expectedDates);
            }
        }

        public class ValidateRollStrategy
        {
            private ByDayOfYear _sut;
            private IClock _clock;
            private IEnumerable<IDate> _dates;

            [Fact]
            public void Execute()
            {
                const int yearLeap = 2106;
                const YearMonth.MonthValue baseMonth = YearMonth.MonthValue.May;
                const int baseDay = 01;

                var fakeClock = ScheduleTestHelper.GetFakeClock(yearLeap, baseMonth, baseDay);

                this.WithExamples(new ExampleTable("SUT", "clock", "Expected Dates")
                    {
                        {
                            new ByDayOfYear.Builder
                            {
                                DayOfYear = 30,
                                Month = YearMonth.MonthValue.February,
                                CountFrom = -02,
                                CountTo = 12,
                                RollStrategy = RepeatingDay.RollStrategyType.Forward
                            }.Build(),
                            fakeClock,
                            Enumerable.Range(yearLeap - 02, 15)
                                .Select(year => new Date(year, YearMonth.MonthValue.March, 01))
                        },

                        {
                            new ByDayOfYear.Builder
                            {
                                DayOfYear = 31,
                                Month = YearMonth.MonthValue.April,
                                CountFrom = -02,
                                CountTo = 12,
                                RollStrategy = RepeatingDay.RollStrategyType.Forward
                            }.Build(),
                            fakeClock,
                            Enumerable.Range(yearLeap - 02, 15)
                                .Select(year => new Date(year, YearMonth.MonthValue.May, 01))
                        },

                        {
                            new ByDayOfYear.Builder
                            {
                                DayOfYear = 29,
                                Month = YearMonth.MonthValue.February,
                                CountFrom = -02,
                                CountTo = 02,
                                RollStrategy = RepeatingDay.RollStrategyType.Forward
                            }.Build(),
                            fakeClock,
                            new List<IDate>
                            {
                                new Date(yearLeap - 02, YearMonth.MonthValue.February, 29),
                                new Date(yearLeap - 01, YearMonth.MonthValue.March, 01),
                                new Date(yearLeap + 00, YearMonth.MonthValue.March, 01),
                                new Date(yearLeap + 01, YearMonth.MonthValue.March, 01),
                                new Date(yearLeap + 02, YearMonth.MonthValue.February, 29),
                            }
                        },

                        {
                            new ByDayOfYear.Builder
                            {
                                DayOfYear = 30,
                                Month = YearMonth.MonthValue.February,
                                CountFrom = -02,
                                CountTo = 12,
                                RollStrategy = RepeatingDay.RollStrategyType.Forward
                            }.Build(),
                            fakeClock,
                            Enumerable.Range(yearLeap - 02, 15)
                                .Select(year => new Date(year, YearMonth.MonthValue.March, 01))
                        },

                        {
                            new ByDayOfYear.Builder
                            {
                                DayOfYear = 29,
                                Month = YearMonth.MonthValue.February,
                                CountFrom = -02,
                                CountTo = 02,
                                RollStrategy = RepeatingDay.RollStrategyType.Back
                            }.Build(),
                            fakeClock,
                            new List<IDate>
                            {
                                new Date(yearLeap - 02, YearMonth.MonthValue.February, 29),
                                new Date(yearLeap - 01, YearMonth.MonthValue.February, 28),
                                new Date(yearLeap + 00, YearMonth.MonthValue.February, 28),
                                new Date(yearLeap + 01, YearMonth.MonthValue.February, 28),
                                new Date(yearLeap + 02, YearMonth.MonthValue.February, 29),
                            }
                        },

                        {
                            new ByDayOfYear.Builder
                            {
                                DayOfYear = 30,
                                Month = YearMonth.MonthValue.February,
                                CountFrom = -02,
                                CountTo = 02,
                                RollStrategy = RepeatingDay.RollStrategyType.Back
                            }.Build(),
                            fakeClock,
                            new List<IDate>
                            {
                                new Date(yearLeap - 02, YearMonth.MonthValue.February, 29),
                                new Date(yearLeap - 01, YearMonth.MonthValue.February, 28),
                                new Date(yearLeap + 00, YearMonth.MonthValue.February, 28),
                                new Date(yearLeap + 01, YearMonth.MonthValue.February, 28),
                                new Date(yearLeap + 02, YearMonth.MonthValue.February, 29),
                            }
                        },

                        {
                            new ByDayOfYear.Builder
                            {
                                DayOfYear = 31,
                                Month = YearMonth.MonthValue.April,
                                CountFrom = -02,
                                CountTo = 02,
                                RollStrategy = RepeatingDay.RollStrategyType.Back
                            }.Build(),
                            fakeClock,
                            new List<IDate>
                            {
                                new Date(yearLeap - 02, YearMonth.MonthValue.April, 30),
                                new Date(yearLeap - 01, YearMonth.MonthValue.April, 30),
                                new Date(yearLeap + 00, YearMonth.MonthValue.April, 30),
                                new Date(yearLeap + 01, YearMonth.MonthValue.April, 30),
                                new Date(yearLeap + 02, YearMonth.MonthValue.April, 30),
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
                _dates = _sut.Generate(_clock);
            }

            public void ThenAllDatesShouldBeThese(IEnumerable<IDate> expectedDates)
            {
                _dates
                    .ShouldBe(expectedDates);
            }
        }
    }
}