﻿using Scheduler.ScheduleInstances;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodaTime;
using TestStack.BDDfy;
using Xunit;

namespace Scheduler.Test.Repeating
{
    [TestClass]
    public class ByDayOfMonthTests
    {
        public class ValidateRollStrategy
        {
            private ByDayOfMonth _sut;
            private IClock _clock;
            private IEnumerable<IDate> _dates;

            [Fact]
            public void Execute()
            {
                const int yearLeap = 2106;
                const YearMonth.MonthValue baseMonth = YearMonth.MonthValue.March;
                const int baseDay = 01;

                var fakeClock = ScheduleTestHelper.GetFakeClock(yearLeap, baseMonth, baseDay);

                this.WithExamples(new ExampleTable("sut", "clock", "Expected Dates")
                    {
                        {
                            new ByDayOfMonth
                            {
                                DayOfMonth = 28,
                                CountFrom = -02,
                                CountTo = 00,
                                RollStrategy = ScheduleAbstracts.RepeatingDay.RollStrategyType.Forward
                            },
                            fakeClock,
                            Enumerable.Range(0, 03)
                                .Select(i => new Date(yearLeap, baseMonth, 28).PlusMonths(-2 + i))
                        },
                        {
                            new ByDayOfMonth
                            {
                                DayOfMonth = 30,
                                CountFrom = -02,
                                CountTo = 02,
                                RollStrategy = ScheduleAbstracts.RepeatingDay.RollStrategyType.Back
                            },
                            fakeClock,
                            new List<IDate>
                            {
                                new Date(yearLeap, YearMonth.MonthValue.January, 30),
                                new Date(yearLeap, YearMonth.MonthValue.February, 28),
                                new Date(yearLeap, YearMonth.MonthValue.March, 30),
                                new Date(yearLeap, YearMonth.MonthValue.April, 30),
                                new Date(yearLeap, YearMonth.MonthValue.May, 30),
                            }
                        },
                        {
                            new ByDayOfMonth
                            {
                                DayOfMonth = 29,
                                CountFrom = -02,
                                CountTo = 02,
                                RollStrategy = ScheduleAbstracts.RepeatingDay.RollStrategyType.Skip
                            },
                            fakeClock,
                            new List<IDate>
                            {
                                new Date(yearLeap, YearMonth.MonthValue.January, 29),
                                new Date(yearLeap, YearMonth.MonthValue.March, 29),
                                new Date(yearLeap, YearMonth.MonthValue.April, 29),
                                new Date(yearLeap, YearMonth.MonthValue.May, 29),
                            }
                        },
                        {
                            new ByDayOfMonth
                            {
                                DayOfMonth = 29,
                                CountFrom = -02,
                                CountTo = 02,
                                RollStrategy = ScheduleAbstracts.RepeatingDay.RollStrategyType.Forward
                            },
                            fakeClock,
                            new List<IDate>
                            {
                                new Date(yearLeap, YearMonth.MonthValue.January, 29),
                                new Date(yearLeap, YearMonth.MonthValue.March, 01),
                                new Date(yearLeap, YearMonth.MonthValue.March, 29),
                                new Date(yearLeap, YearMonth.MonthValue.April, 29),
                                new Date(yearLeap, YearMonth.MonthValue.May, 29),
                            }
                        },
                    })
                    .BDDfy();
            }

            public void GivenAByDayOfMonth(ByDayOfMonth sut)
            {
                _sut = sut;
            }

            public void AndGivenAClock(IClock clock)
            {
                _clock = clock;
            }

            public void WhenDatesAreRetrieved()
            {
                _dates = _sut
                    .Generate(_clock);
            }

            public void ThenAllDatesShouldBeExpected(
                IEnumerable<IDate> expectedDates)
            {
                _dates
                    .Select(d => d.Value)
                    .ShouldBe(expectedDates.Select(e => e.Value));
            }
        }
    }
}