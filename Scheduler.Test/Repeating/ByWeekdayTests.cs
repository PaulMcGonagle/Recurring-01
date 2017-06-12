using System.Collections.Generic;
using System.Linq;
using NodaTime;
using Scheduler.ScheduleInstances;
using Shouldly;
using TestStack.BDDfy;
using Xunit;

namespace Scheduler.Test.Repeating
{
    public class ByWeekdayTests
    {
        public class VerifyDates
        {
            private ByWeekday _sut;
            private IClock _clock;
            private IEnumerable<IDate> _generatedDates;

            [Fact]
            public void Execute()
            {
                var fakeClock = ScheduleTestHelper.GetFakeClock(2016, 05, 01);

                this.WithExamples(table: new ExampleTable("repeatingDay", "dayOfWeek", "firstDate", "clock", "lastDate")
                    {
                        {   new ByWeekday(weekday: IsoDayOfWeek.Monday), IsoDayOfWeek.Monday, new Date(2016, YearMonth.MonthValue.May, 02), fakeClock, new Date(2017, YearMonth.MonthValue.May, 01) },
                        //{   new ByWeekday(weekday: IsoDayOfWeek.Tuesday), IsoDayOfWeek.Tuesday, new Date(2016, YearMonth.MonthValue.April, 26), fakeClock, new Date(2017, YearMonth.MonthValue.April, 25) },
                        //{   new ByWeekday(weekday: IsoDayOfWeek.Wednesday), IsoDayOfWeek.Wednesday, new Date(2016, YearMonth.MonthValue.April, 27), fakeClock, new Date(2017, YearMonth.MonthValue.April, 26) },
                        //{   new ByWeekday(weekday: IsoDayOfWeek.Thursday), IsoDayOfWeek.Thursday, new Date(2016, YearMonth.MonthValue.April, 28), fakeClock, new Date(2017, YearMonth.MonthValue.April, 27) },
                        //{   new ByWeekday(weekday: IsoDayOfWeek.Friday), IsoDayOfWeek.Friday, new Date(2016, YearMonth.MonthValue.April, 29), fakeClock, new Date(2017, YearMonth.MonthValue.April, 28) },
                        //{   new ByWeekday(weekday: IsoDayOfWeek.Saturday), IsoDayOfWeek.Saturday, new Date(2016, YearMonth.MonthValue.April, 30), fakeClock, new Date(2017, YearMonth.MonthValue.April, 29) },
                        //{   new ByWeekday(weekday: IsoDayOfWeek.Sunday), IsoDayOfWeek.Sunday, new Date(2016, YearMonth.MonthValue.May, 01), fakeClock, new Date(2017, YearMonth.MonthValue.April, 30) },
                    })
                    .BDDfy();
            }

            public void GivenARepeatingDay(ByWeekday repeatingDay)
            {
                _sut = repeatingDay;
            }

            public void AndGivenAClock(IClock clock)
            {
                _clock = clock;
            }

            public void WhenDatesAreGenerated()
            {
                _generatedDates = _sut.Generate(_clock);
            }

            public void ThenAllDatesShouldBeThisDay(IsoDayOfWeek dayOfWeek)
            {
                _generatedDates
                    .Select(date => date.IsoDayOfWeek)
                    .ShouldAllBe(date => date == dayOfWeek);
            }

            public void AndThenTheFirstDateShouldBeThis(Date firstDate)
            {
                _generatedDates
                    .Select(date => date)
                    .Min()
                    .ShouldBe(firstDate);
            }

            public void AndThenTheLastDateShouldBeThis(Date lastDate)
            {
                _generatedDates
                    .Select(date => date)
                    .Max()
                    .ShouldBe(lastDate);
            }
        }
    }
}