using System.Collections.Generic;
using System.Linq;
using Scheduler.ScheduleInstances;
using Shouldly;
using NodaTime;
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
            private IEnumerable<Scheduler.Date> _dates;

            [Fact]
            public void Execute()
            {
                this.WithExamples(new ExampleTable("sut", "daysOfWeek", "firstDate", "lastDate")
                    {
                        {
                            new ByWeekdays
                            {
                                Days = new List<IsoDayOfWeek> {IsoDayOfWeek.Saturday, IsoDayOfWeek.Sunday},
                                EdgeRange = new EdgeRange(2000, YearMonth.MonthValue.April, 15, 2010, YearMonth.MonthValue.November, 28),
                            },
                            new List<IsoDayOfWeek> {IsoDayOfWeek.Saturday, IsoDayOfWeek.Sunday},
                            new Scheduler.Date(2000, YearMonth.MonthValue.April, 15),
                            new Scheduler.Date(2010, YearMonth.MonthValue.November, 28)
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
                _dates = _sut.Dates;
            }

            public void ThenAllDatesShouldBeThisDay(List<IsoDayOfWeek> daysOfWeek)
            {
                _dates
                    .Select(d => d.Value.DayOfWeek)
                    .ShouldBeSubsetOf(daysOfWeek.Select(d => (int) d));
            }

            public void AndThenTheFirstDateShouldBeThis(Scheduler.Date firstDate)
            {
                _dates
                    .Min()
                    .ShouldBe(firstDate);
            }

            public void AndThenTheLastDateShouldBeThis(Scheduler.Date lastDate)
            {
                _dates
                    .Max()
                    .ShouldBe(lastDate);
            }
        }
    }
}