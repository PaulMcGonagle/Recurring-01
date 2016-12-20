using NodaTime;
using Scheduler.ScheduleInstances;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using TestStack.BDDfy;
using Xunit;

namespace Scheduler.Test.Repeating
{
    public class ByDayOfMonthTests
    {
        public class ValidateRollStrategy
        {
            private ByDayOfMonth _sut;
            private IEnumerable<Scheduler.Date> _dates;

            [Fact]
            public void Execute()
            {
                const int yearLeap = 2106;
                const YearMonth.MonthValue baseMonth = YearMonth.MonthValue.March;
                const int baseDay = 01;

                var fakeClock = ScheduleTestHelper.GetFakeClock(yearLeap, baseMonth, baseDay);

                this.WithExamples(new ExampleTable("SUT", "Expected Dates")
                    {
                        {
                            new ByDayOfMonth
                            {
                                DayOfMonth = 28,
                                Clock = fakeClock,
                                CountFrom = -02,
                                CountTo = 00,
                                RollStrategy = ScheduleAbstracts.RepeatingDay.RollStrategyType.Forward
                            },
                            Enumerable.Range(0, 03)
                                .Select(i => new Scheduler.Date(yearLeap, baseMonth, 28).PlusMonths(-2 + i))
                        },

                        {
                            new ByDayOfMonth
                            {
                                DayOfMonth = 30,
                                Clock = fakeClock,
                                CountFrom = -02,
                                CountTo = 02,
                                RollStrategy = ScheduleAbstracts.RepeatingDay.RollStrategyType.Back
                            },
                            new List<Scheduler.Date>
                            {
                                new Scheduler.Date(yearLeap, YearMonth.MonthValue.January, 30),
                                new Scheduler.Date(yearLeap, YearMonth.MonthValue.February, 28),
                                new Scheduler.Date(yearLeap, YearMonth.MonthValue.March, 30),
                                new Scheduler.Date(yearLeap, YearMonth.MonthValue.April, 30),
                                new Scheduler.Date(yearLeap, YearMonth.MonthValue.May, 30),
                            }
                        },

                        {
                            new ByDayOfMonth
                            {
                                DayOfMonth = 29,
                                Clock = fakeClock,
                                CountFrom = -02,
                                CountTo = 02,
                                RollStrategy = ScheduleAbstracts.RepeatingDay.RollStrategyType.Skip
                            },
                            new List<Scheduler.Date>
                            {
                                new Scheduler.Date(yearLeap, YearMonth.MonthValue.January, 29),
                                new Scheduler.Date(yearLeap, YearMonth.MonthValue.March, 29),
                                new Scheduler.Date(yearLeap, YearMonth.MonthValue.April, 29),
                                new Scheduler.Date(yearLeap, YearMonth.MonthValue.May, 29),
                            }
                        },

                        {
                            new ByDayOfMonth
                            {
                                DayOfMonth = 29,
                                Clock = fakeClock,
                                CountFrom = -02,
                                CountTo = 02,
                                RollStrategy = ScheduleAbstracts.RepeatingDay.RollStrategyType.Forward
                            },
                            new List<Scheduler.Date>
                            {
                                new Scheduler.Date(yearLeap, YearMonth.MonthValue.January, 29),
                                new Scheduler.Date(yearLeap, YearMonth.MonthValue.March, 01),
                                new Scheduler.Date(yearLeap, YearMonth.MonthValue.March, 29),
                                new Scheduler.Date(yearLeap, YearMonth.MonthValue.April, 29),
                                new Scheduler.Date(yearLeap, YearMonth.MonthValue.May, 29),
                            }
                        },
                    })
                    .BDDfy();
            }

            public void GivenAByDayOfMonth(ByDayOfMonth sut)
            {
                _sut = sut;
            }

            public void WhenDatesAreRetrieved()
            {
                _dates = _sut.Dates;
            }

            public void ThenAllDatesShouldBeExpected(
                IEnumerable<Scheduler.Date> expectedDates)
            {
                _dates
                    .Select(d => d.Value)
                    .ShouldBe(expectedDates.Select(e => e.Value));
            }
        }
    }
}