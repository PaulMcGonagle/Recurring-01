using System.Collections.Generic;
using System.Linq;
using NodaTime;
using NodaTime.Testing;
using Scheduler.Persistance;
using Scheduler.ScheduleInstances;
using Shouldly;
using TestStack.BDDfy;
using Xunit;

namespace Scheduler.Test
{
    public class CompositeScheduleTests
    {
        public class VerifyExclusionsAreExcluded
        {
            private CompositeSchedule _sut;
            private IClock _clock;
            private IEnumerable<IDate> _dates;

            [Fact]
            public void Execute()
            {
                this.WithExamples(new ExampleTable("sut", "clock", "expectedFirstDate", "expectedLastDate", "excludedIsoDayOfWeeks")
                    {
                        {
                            new CompositeSchedule
                            {
                                Inclusions = new EdgeVertexs<ISchedule>
                                {
                                    new EdgeVertex<ISchedule>(new Schedule
                                    {
                                        ScheduleInstance = new ByDateList.Builder
                                        {
                                            Items = new EdgeVertexs<IDate>(
                                                DateTimeHelper.Range
                                                (
                                                    start: new Date(2016, YearMonth.MonthValue.January, 01),
                                                    end: new Date(2016, YearMonth.MonthValue.January, 31)
                                                ).Where(d => d.IsoDayOfWeek == IsoDayOfWeek.Monday).ToList()
                                            ),
                                        }.Build(),
                                    }),
                                },
                                Exclusions = new EdgeVertexs<ISchedule>()
                                {
                                    new EdgeVertex<ISchedule>(new Schedule
                                    {
                                        ScheduleInstance = new ByDateList.Builder
                                        {
                                            Items = new EdgeVertexs<IDate>(
                                                DateTimeHelper.Range
                                                (
                                                    start: new Date(2016, YearMonth.MonthValue.January, 01),
                                                    end: new Date(2016, YearMonth.MonthValue.January, 15)
                                                ).Where(d => d.IsoDayOfWeek == IsoDayOfWeek.Monday).ToList()
                                            ),
                                        }.Build(),
                                    }),
                                },
                            },
                            new FakeClock(Instant.FromUtc(2017, 04, 02, 03, 30, 00)),
                            new Date(2016, YearMonth.MonthValue.January, 18),
                            new Date(2016, YearMonth.MonthValue.January, 25),
                            new List<IsoDayOfWeek>
                            {
                                IsoDayOfWeek.Monday,
                            }
                        }
                    })
                    .BDDfy();
            }

            public void GivenACompositeSchedule(CompositeSchedule sut)
            {
                _sut = sut;
            }

            public void WhenDatesAreRetrieved()
            {
                _dates = _sut.Generate(_clock);
            }

            public void ThenTheFirstDateIs(Date expectedFirstDate)
            {
                _dates
                    .Select(date => date.Value)
                    .Min()
                    .ShouldBe(expectedFirstDate.Value);
            }

            public void AndThenTheLastDateIs(Date expectedLastDate)
            {
                var localDates = _dates
                    .Select(date => date.Value);

                var max = localDates
                    .Max();

                max.ShouldBe(expectedLastDate.Value);
            }

            public void AndTheseDaysShouldNotAppear(IEnumerable<IsoDayOfWeek> excludedIsoDayOfWeeks)
            {
                _dates
                    .Select(date => date.IsoDayOfWeek)
                    .ShouldNotBeOneOf(excludedIsoDayOfWeeks);
            }
        }

    }
}
