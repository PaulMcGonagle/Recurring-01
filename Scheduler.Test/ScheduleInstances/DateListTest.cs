using System.Collections.Generic;
using System.Linq;
using Scheduler.Generation;
using Shouldly;
using TestStack.BDDfy;
using Scheduler.ScheduleInstances;
using Xunit;

namespace Scheduler.Test.ScheduleInstances
{
    public class DateListTests
    {
        public class VerifyDay
        {
            private DateList _sut;
            private IEnumerable<IGeneratedDate> _dates;

            [Fact]
            public void Execute()
            {
                this.WithExamples(new ExampleTable("sut", "expectedDates")
                    {
                        {
                            new DateList
                            {
                                Items = new List<Date> {new Date(2015, YearMonth.MonthValue.March, 17), new Date(2016, YearMonth.MonthValue.April, 25)}
                            },
                            new List<Date> {new Date(2015, YearMonth.MonthValue.March, 17), new Date(2016, YearMonth.MonthValue.April, 25)}
                        },
                        {
                            new DateList {Items = DateTimeHelper.Range(new Date(2014, YearMonth.MonthValue.April, 28), 20)},
                            DateTimeHelper.Range(new Date(2014, YearMonth.MonthValue.April, 28), 20)
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
                _dates = _sut.Generate();
            }

            public void ThenOnlyTheseDateAreReturned(IEnumerable<Date> expectedDates)
            {
                _dates.Select(d => d.Date.Value).ShouldBe(expectedDates.Select(e => e.Value));
            }
        }
    }
}