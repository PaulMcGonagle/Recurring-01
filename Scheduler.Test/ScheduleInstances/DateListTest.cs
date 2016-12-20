using System.Collections.Generic;
using System.Linq;
using Shouldly;
using NodaTime;
using TestStack.BDDfy;
using Scheduler.ScheduleInstances;
using Xunit;

namespace Scheduler.Test.ScheduleInstances
{
    public class DateListTests
    {
        public class VerifyDay
        {
            DateList _sut;
            IEnumerable<Scheduler.Date> _dates;

            [Fact]
            public void Execute()
            {
                this.WithExamples(new ExampleTable("sut", "expectedDates")
                    {
                        {
                            new DateList
                            {
                                Items = new List<Scheduler.Date> {new Scheduler.Date(2015, YearMonth.MonthValue.March, 17), new Scheduler.Date(2016, YearMonth.MonthValue.April, 25)}
                            },
                            new List<Scheduler.Date> {new Scheduler.Date(2015, YearMonth.MonthValue.March, 17), new Scheduler.Date(2016, YearMonth.MonthValue.April, 25)}
                        },
                        {
                            new DateList {Items = DateTimeHelper.Range(new Scheduler.Date(2014, YearMonth.MonthValue.April, 28), 20)},
                            DateTimeHelper.Range(new Scheduler.Date(2014, YearMonth.MonthValue.April, 28), 20)
                        },
                    })
                    .BDDfy();
            }

            public void GivenADateList(DateList sut)
            {
                _sut = sut;
            }

            public void WhenDatesAreRetrieved()
            {
                _dates = _sut.Dates;
            }

            public void ThenOnlyTheseDateAreReturned(IEnumerable<Scheduler.Date> expectedDates)
            {
                _dates.Select(d => d.Value).ShouldBe(expectedDates.Select(e => e.Value));
            }
        }
    }
}