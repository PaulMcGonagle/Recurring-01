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
                            new DateList
                            {
                                Items = new List<LocalDate> {DateTimeHelper.GetLocalDate(2015, YearMonth.MonthValue.March, 17), DateTimeHelper.GetLocalDate(2016, YearMonth.MonthValue.April, 25)}
                            },
                            new List<LocalDate> {DateTimeHelper.GetLocalDate(2015, YearMonth.MonthValue.March, 17), DateTimeHelper.GetLocalDate(2016, YearMonth.MonthValue.April, 25)}
                        },
                        {
                            new DateList {Items = DateTimeHelper.Range(DateTimeHelper.GetLocalDate(2014, YearMonth.MonthValue.April, 28), 20)},
                            DateTimeHelper.Range(DateTimeHelper.GetLocalDate(2014, YearMonth.MonthValue.April, 28), 20)
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