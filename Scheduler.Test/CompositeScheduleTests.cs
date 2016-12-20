using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodaTime;
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
            private IEnumerable<Scheduler.Date> _dates;

            [Fact]
            public void Execute()
            {
                this.WithExamples(new ExampleTable("sut", "expectedFirstDate", "expectedLastDate", "excludedIsoDayOfWeeks")
                    {
                        {
                            new CompositeSchedule
                            {
                                Inclusions = new List<ISchedule>
                                {
                                    new DateList
                                    {
                                        Items =
                                            DateTimeHelper.Range(
                                                new Scheduler.Date(2016, YearMonth.MonthValue.January, 01),
                                                new Scheduler.Date(2018, YearMonth.MonthValue.December, 31)
                                            ).ToList(),
                                    },
                                },
                                Exclusions = new List<ISchedule>
                                {
                                    new DateList
                                    {
                                        Items =
                                            DateTimeHelper.Range(
                                                new Scheduler.Date(2016, YearMonth.MonthValue.January, 01),
                                                new Scheduler.Date(2018, YearMonth.MonthValue.December, 31)
                                            )
                                            .Where(d => d.IsoDayOfWeek == IsoDayOfWeek.Monday).ToList(),
                                    },
                                },
                            },
                            new Scheduler.Date(2016, YearMonth.MonthValue.January, 01),
                            new Scheduler.Date(2018, YearMonth.MonthValue.December, 30),
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
                _dates = _sut.Dates;
            }

            public void ThenTheFirstDateIs(Scheduler.Date expectedFirstDate)
            {
                _dates
                    .Select(d => d.Value)
                    .Min()
                    .ShouldBe(expectedFirstDate.Value);
            }

            public void AndThenTheLastDateIs(Scheduler.Date expectedLastDate)
            {
                _dates
                    .Select(d => d.Value)
                    .Max()
                    .ShouldBe(expectedLastDate.Value);
            }

            public void AndTheseDaysShouldNotAppear(IEnumerable<IsoDayOfWeek> excludedIsoDayOfWeeks)
            {
                _dates.Select(d => d.IsoDayOfWeek).ShouldNotBeOneOf(excludedIsoDayOfWeeks);
            }
        }

    }
}
