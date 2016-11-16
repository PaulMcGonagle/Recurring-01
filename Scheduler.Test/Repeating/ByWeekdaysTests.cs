using System.Collections.Generic;
using System.Linq;
using Scheduler.ScheduleInstances;
using Shouldly;
using NodaTime;
using TestStack.BDDfy;
using Xunit;

namespace Scheduler.Test.Repeating
{
    public class ByWeekdaysTests
    {
        public class VerifyOccurrences
        {
            private ByWeekdays _sut;
            private IEnumerable<LocalDate> _occurrences;

            [Fact]
            public void Execute()
            {
                this.WithExamples(new ExampleTable("sut", "daysOfWeek", "firstDate", "lastDate")
                    {
                        {
                            new ByWeekdays()
                            {
                                Days = new List<IsoDayOfWeek>() {IsoDayOfWeek.Saturday, IsoDayOfWeek.Sunday},
                                DateFrom = DateTimeHelper.GetLocalDate(2000, YearMonth.MonthValue.April, 15),
                                DateTo = DateTimeHelper.GetLocalDate(2010, YearMonth.MonthValue.November, 28),
                            },
                            new List<IsoDayOfWeek>() {IsoDayOfWeek.Saturday, IsoDayOfWeek.Sunday},
                            DateTimeHelper.GetLocalDate(2000, YearMonth.MonthValue.April, 15),
                            DateTimeHelper.GetLocalDate(2010, YearMonth.MonthValue.November, 28)
                        },
                    })
                    .BDDfy();
            }

            public void GivenARepeatingDay(ByWeekdays sut)
            {
                _sut = sut;
            }

            public void WhenOccurrencesAreRetrieved()
            {
                _occurrences = _sut.Occurrences();
            }

            public void ThenAllOccurrencesShouldBeThisDay(List<IsoDayOfWeek> daysOfWeek)
            {
                _occurrences
                    .Select(o => o.DayOfWeek)
                    .ShouldBeSubsetOf(daysOfWeek.Select(o => (int) o));
            }

            public void AndThenTheFirstDateShouldBeThis(LocalDate firstDate)
            {
                _occurrences
                    .Min()
                    .ShouldBe(firstDate);
            }

            public void AndThenTheLastDateShouldBeThis(LocalDate lastDate)
            {
                _occurrences
                    .Max()
                    .ShouldBe(lastDate);
            }
        }
    }
}