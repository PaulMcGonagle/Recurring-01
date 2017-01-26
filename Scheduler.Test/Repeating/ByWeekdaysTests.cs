using System.Collections.Generic;
using System.Linq;
using Scheduler.ScheduleInstances;
using Shouldly;
using NodaTime;
using Scheduler.Generation;
using Scheduler.ScheduleEdges;
using TestStack.BDDfy;
using Xunit;

namespace Scheduler.Test.Repeating
{
    public class ByWeekdaysTests
    {
        public class VerifyDates
        {
            private ByWeekdays _sut;
            private IEnumerable<IGeneratedDate> _dates;

            [Fact]
            public void Execute()
            {
                this.WithExamples(new ExampleTable("sut", "daysOfWeek", "firstDate", "lastDate")
                    {
                        {
                            new ByWeekdays
                            {
                                Days = new List<IsoDayOfWeek> {IsoDayOfWeek.Saturday, IsoDayOfWeek.Sunday},
                                EdgeRange = new EdgeRangeDate(2000, YearMonth.MonthValue.April, 15, 2010, YearMonth.MonthValue.November, 28),
                            },
                            new List<IsoDayOfWeek> {IsoDayOfWeek.Saturday, IsoDayOfWeek.Sunday},
                            new Date(2000, YearMonth.MonthValue.April, 15),
                            new Date(2010, YearMonth.MonthValue.November, 28)
                        },
                    })
                    .BDDfy();
            }

            public void GivenARepeatingDay(ByWeekdays sut)
            {
                _sut = sut;
            }

            public void WhenDatesAreRetrieved()
            {
                _dates = _sut.Generate();
            }

            public void ThenAllDatesShouldBeThisDay(List<IsoDayOfWeek> daysOfWeek)
            {
                _dates
                    .Select(d => d.Date.Value.DayOfWeek)
                    .ShouldBeSubsetOf(daysOfWeek.Select(d => (int) d));
            }

            public void AndThenTheFirstDateShouldBeThis(Date firstDate)
            {
                _dates
                    .Select(d => d.Date)
                    .Min()
                    .ShouldBe(firstDate);
            }

            public void AndThenTheLastDateShouldBeThis(Date lastDate)
            {
                _dates
                    .Select(d => d.Date)
                    .Max()
                    .ShouldBe(lastDate);
            }
        }
    }
}