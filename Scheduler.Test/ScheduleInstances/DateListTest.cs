using System.Collections.Generic;
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
            IEnumerable<LocalDate> _dates;

            [Fact]
            public void Execute()
            {
                this.WithExamples(new ExampleTable("sut", "expectedDates")
                    {
                        {
                            new DateList()
                            {
                                Items = new List<LocalDate>() {new LocalDate(2015, 03, 17), new LocalDate(2016, 04, 25)}
                            },
                            new List<LocalDate>() {new LocalDate(2015, 03, 17), new LocalDate(2016, 04, 25)}
                        },
                        {
                            new DateList() {Items = DateTimeHelper.Range(new LocalDate(2014, 04, 28), 20)},
                            DateTimeHelper.Range(new LocalDate(2014, 04, 28), 20)
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
                _dates = _sut.Dates();
            }

            public void ThenOnlyTheseDateAreReturned(IEnumerable<LocalDate> expectedDates)
            {
                _dates.ShouldBe(expectedDates);
            }
        }
    }
}