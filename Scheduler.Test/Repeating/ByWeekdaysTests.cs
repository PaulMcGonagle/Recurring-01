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
            private IClock _clock;
            private IEnumerable<IDate> _dates;

            [Fact]
            public void Execute()
            {
                var fakeClock = ScheduleTestHelper.GetFakeClock(2016, YearMonth.MonthValue.March, 15);

                this.WithExamples(new ExampleTable("sut", "clock", "daysOfWeek", "firstDate", "lastDate")
                    {
                        {
                            new ByWeekdays
                            {
                                Weekdays = new List<IsoDayOfWeek> {IsoDayOfWeek.Saturday, IsoDayOfWeek.Sunday},
                                EdgeRangeDate = new EdgeRangeDate(new Date(2000, YearMonth.MonthValue.April, 15), new Date(2010, YearMonth.MonthValue.November, 28)),
                            },
                            fakeClock,
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
                _dates = _sut.Generate(_clock);
            }

            public void ThenAllDatesShouldBeThisDay(List<IsoDayOfWeek> daysOfWeek)
            {
                _dates
                    .Select(date => date.Value.DayOfWeek)
                    .ShouldBeSubsetOf(daysOfWeek.Select(d => (int) d));
            }

            public void AndThenTheFirstDateShouldBeThis(Date firstDate)
            {
                _dates
                    .Select(date => date)
                    .Min()
                    .ShouldBe(firstDate);
            }

            public void AndThenTheLastDateShouldBeThis(Date lastDate)
            {
                _dates
                    .Select(date => date)
                    .Max()
                    .ShouldBe(lastDate);
            }
        }
    }
}