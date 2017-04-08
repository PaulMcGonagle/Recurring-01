using System.Collections.Generic;
using System.Linq;
using NodaTime;
using Scheduler.Generation;
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
            CompositeSchedule _sut;
            private IEnumerable<IDate> _dates;

            [Fact]
            public void Execute()
            {
                this.WithExamples(new ExampleTable("sut", "expectedFirstDate", "expectedLastDate", "excludedIsoDayOfWeeks")
                    {
                        {
                            new CompositeSchedule
                            {
                                InclusionsEdges = new EdgeVertexs<ISchedule>()
                                {
                                    new EdgeVertex<ISchedule>(new DateList
                                        {
                                            Items =
                                                DateTimeHelper.Range(
                                                    new Date(2016, YearMonth.MonthValue.January, 01),
                                                    new Date(2018, YearMonth.MonthValue.December, 31)
                                                ).ToList(),
                                        })
                                },
                                ExclusionsEdges = new EdgeVertexs<ISchedule>()
                                {
                                    new EdgeVertex<ISchedule>(new DateList
                                        {
                                            Items =
                                            DateTimeHelper.Range(
                                                new Date(2016, YearMonth.MonthValue.January, 01),
                                                new Date(2018, YearMonth.MonthValue.December, 31)
                                            )
                                            .Where(d => d.IsoDayOfWeek == IsoDayOfWeek.Monday).ToList(),
                                        })
                                },
                            },
                            new Date(2016, YearMonth.MonthValue.January, 01),
                            new Date(2018, YearMonth.MonthValue.December, 30),
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
                _dates = _sut.Generate();
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
