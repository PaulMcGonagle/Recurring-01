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
            private IEnumerable<LocalDate> _occurrences;

            [Fact]
            public void Execute()
            {
                var yearLeap = 2106;
                var baseMonth = YearMonth.MonthValue.March;
                var baseDay = 01;

                var fakeClock = ScheduleTestHelper.GetFakeClock(yearLeap, baseMonth, baseDay);

                this.WithExamples(new ExampleTable("SUT", "Expected Dates")
                    {
                        {
                            new ByDayOfMonth()
                            {
                                DayOfMonth = 28,
                                Clock = fakeClock,
                                CountFrom = -02,
                                CountTo = 00,
                                RollStrategy = ScheduleAbstracts.RepeatingDay.RollStrategyType.Forward
                            },
                            Enumerable.Range(0, 03)
                                .Select(o => DateTimeHelper.GetLocalDate(yearLeap, baseMonth, 28).PlusMonths(-2 + o))
                        },

                        {
                            new ByDayOfMonth()
                            {
                                DayOfMonth = 30,
                                Clock = fakeClock,
                                CountFrom = -02,
                                CountTo = 02,
                                RollStrategy = ScheduleAbstracts.RepeatingDay.RollStrategyType.Back
                            },
                            new List<LocalDate>()
                            {
                                DateTimeHelper.GetLocalDate(yearLeap, YearMonth.MonthValue.January, 30),
                                DateTimeHelper.GetLocalDate(yearLeap, YearMonth.MonthValue.February, 28),
                                DateTimeHelper.GetLocalDate(yearLeap, YearMonth.MonthValue.March, 30),
                                DateTimeHelper.GetLocalDate(yearLeap, YearMonth.MonthValue.April, 30),
                                DateTimeHelper.GetLocalDate(yearLeap, YearMonth.MonthValue.May, 30),
                            }
                        },

                        {
                            new ByDayOfMonth()
                            {
                                DayOfMonth = 29,
                                Clock = fakeClock,
                                CountFrom = -02,
                                CountTo = 02,
                                RollStrategy = ScheduleAbstracts.RepeatingDay.RollStrategyType.Skip
                            },
                            new List<LocalDate>()
                            {
                                DateTimeHelper.GetLocalDate(yearLeap, YearMonth.MonthValue.January, 29),
                                DateTimeHelper.GetLocalDate(yearLeap, YearMonth.MonthValue.March, 29),
                                DateTimeHelper.GetLocalDate(yearLeap, YearMonth.MonthValue.April, 29),
                                DateTimeHelper.GetLocalDate(yearLeap, YearMonth.MonthValue.May, 29),
                            }
                        },

                        {
                            new ByDayOfMonth()
                            {
                                DayOfMonth = 29,
                                Clock = fakeClock,
                                CountFrom = -02,
                                CountTo = 02,
                                RollStrategy = ScheduleAbstracts.RepeatingDay.RollStrategyType.Forward
                            },
                            new List<LocalDate>()
                            {
                                DateTimeHelper.GetLocalDate(yearLeap, YearMonth.MonthValue.January, 29),
                                DateTimeHelper.GetLocalDate(yearLeap, YearMonth.MonthValue.March, 01),
                                DateTimeHelper.GetLocalDate(yearLeap, YearMonth.MonthValue.March, 29),
                                DateTimeHelper.GetLocalDate(yearLeap, YearMonth.MonthValue.April, 29),
                                DateTimeHelper.GetLocalDate(yearLeap, YearMonth.MonthValue.May, 29),
                            }
                        },
                    })
                    .BDDfy();
            }

            public void GivenAByDayOfMonth(ByDayOfMonth sut)
            {
                _sut = sut;
            }

            public void WhenOccurrencesAreRetrieved()
            {
                _occurrences = _sut.Occurrences();
            }

            public void ThenAllOccurrencesShouldBeExpected(
                IEnumerable<LocalDate> expectedDates)
            {
                _occurrences
                    .ShouldBeSubsetOf(expectedDates);
            }

            public void AndThenAllExpectedDatesShouldBeInOccurrences(
                IEnumerable<LocalDate> expectedDates)
            {
                expectedDates
                    .ShouldBeSubsetOf(_occurrences);
            }
        }
    }
}