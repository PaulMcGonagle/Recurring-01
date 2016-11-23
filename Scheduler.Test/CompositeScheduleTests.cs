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
            private IEnumerable<LocalDate> _dates;

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
                                                DateTimeHelper.GetLocalDate(2016, YearMonth.MonthValue.January, 01),
                                                DateTimeHelper.GetLocalDate(2018, YearMonth.MonthValue.December, 31)
                                            ).ToList(),
                                    },
                                },
                                Exclusions = new List<ISchedule>
                                {
                                    new DateList
                                    {
                                        Items =
                                            DateTimeHelper.Range(
                                                DateTimeHelper.GetLocalDate(2016, YearMonth.MonthValue.January, 01),
                                                DateTimeHelper.GetLocalDate(2018, YearMonth.MonthValue.December, 31)
                                            )
                                            .Where(d => d.IsoDayOfWeek == IsoDayOfWeek.Monday).ToList(),
                                    },
                                },
                            },
                            DateTimeHelper.GetLocalDate(2016, YearMonth.MonthValue.January, 01),
                            DateTimeHelper.GetLocalDate(2018, YearMonth.MonthValue.December, 30),
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

            public void ThenTheFirstDateIs(LocalDate expectedFirstDate)
            {
                _dates.Min().ShouldBe(expectedFirstDate);
            }

            public void AndThenTheLastDateIs(LocalDate expectedLastDate)
            {
                _dates.Max().ShouldBe(expectedLastDate);
            }

            public void AndTheseDaysShouldNotAppear(IEnumerable<IsoDayOfWeek> excludedIsoDayOfWeeks)
            {
                _dates.Select(d => d.IsoDayOfWeek).ShouldNotBeOneOf(excludedIsoDayOfWeeks);
            }
        }

    }
}
