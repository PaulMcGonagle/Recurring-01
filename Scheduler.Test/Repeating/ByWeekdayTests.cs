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
            private ByWeekday _repeatingDay;
            private IEnumerable<IDate> _generatedDates;

            [Fact]
            public void Execute()
            {
                var fakeClock = ScheduleTestHelper.GetFakeClock(2016, 05, 01);

                this.WithExamples(table: new ExampleTable("repeatingDay", "dayOfWeek", "firstDate", "lastDate")
                    {
                        {   new ByWeekday(clock: fakeClock, weekday: IsoDayOfWeek.Monday), IsoDayOfWeek.Monday, new Date(2016, YearMonth.MonthValue.May, 02), new Date(2017, YearMonth.MonthValue.May, 01) },
                        //{   new ByWeekday(clock: fakeClock, weekday: IsoDayOfWeek.Tuesday), IsoDayOfWeek.Tuesday, new Date(2016, YearMonth.MonthValue.April, 26), new Date(2017, YearMonth.MonthValue.April, 25) },
                        //{   new ByWeekday(clock: fakeClock, weekday: IsoDayOfWeek.Wednesday), IsoDayOfWeek.Wednesday, new Date(2016, YearMonth.MonthValue.April, 27), new Date(2017, YearMonth.MonthValue.April, 26) },
                        //{   new ByWeekday(clock: fakeClock, weekday: IsoDayOfWeek.Thursday), IsoDayOfWeek.Thursday, new Date(2016, YearMonth.MonthValue.April, 28), new Date(2017, YearMonth.MonthValue.April, 27) },
                        //{   new ByWeekday(clock: fakeClock, weekday: IsoDayOfWeek.Friday), IsoDayOfWeek.Friday, new Date(2016, YearMonth.MonthValue.April, 29), new Date(2017, YearMonth.MonthValue.April, 28) },
                        //{   new ByWeekday(clock: fakeClock, weekday: IsoDayOfWeek.Saturday), IsoDayOfWeek.Saturday, new Date(2016, YearMonth.MonthValue.April, 30), new Date(2017, YearMonth.MonthValue.April, 29) },
                        //{   new ByWeekday(clock: fakeClock, weekday: IsoDayOfWeek.Sunday), IsoDayOfWeek.Sunday, new Date(2016, YearMonth.MonthValue.May, 01), new Date(2017, YearMonth.MonthValue.April, 30) },
                    })
                    .BDDfy();
            }

            public void GivenARepeatingDay(ByWeekday repeatingDay)
            {
                _repeatingDay = repeatingDay;
            }

            public void WhenDatesAreGenerated()
            {
                _generatedDates = _repeatingDay.Generate();
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