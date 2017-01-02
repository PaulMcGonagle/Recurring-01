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

            [Fact]
            public void Execute()
            {
                var fakeClock = ScheduleTestHelper.GetFakeClock(2016, 05, 01);

                this.WithExamples(table: new ExampleTable("repeatingDay", "dayOfWeek", "firstDate", "lastDate")
                    {
                        {   new ByWeekday {Weekday = IsoDayOfWeek.Monday, Clock = fakeClock}, IsoDayOfWeek.Monday, new Date(2016, YearMonth.MonthValue.April, 25), new Date(2017, YearMonth.MonthValue.April, 24) },
                        {   new ByWeekday {Weekday = IsoDayOfWeek.Tuesday, Clock = fakeClock}, IsoDayOfWeek.Tuesday, new Date(2016, YearMonth.MonthValue.April, 26), new Date(2017, YearMonth.MonthValue.April, 25) },
                        {   new ByWeekday {Weekday = IsoDayOfWeek.Wednesday, Clock = fakeClock}, IsoDayOfWeek.Wednesday, new Date(2016, YearMonth.MonthValue.April, 27), new Date(2017, YearMonth.MonthValue.April, 26) },
                        {   new ByWeekday {Weekday = IsoDayOfWeek.Thursday, Clock = fakeClock}, IsoDayOfWeek.Thursday, new Date(2016, YearMonth.MonthValue.April, 28), new Date(2017, YearMonth.MonthValue.April, 27) },
                        {   new ByWeekday {Weekday = IsoDayOfWeek.Friday, Clock = fakeClock}, IsoDayOfWeek.Friday, new Date(2016, YearMonth.MonthValue.April, 29), new Date(2017, YearMonth.MonthValue.April, 28) },
                        {   new ByWeekday {Weekday = IsoDayOfWeek.Saturday, Clock = fakeClock}, IsoDayOfWeek.Saturday, new Date(2016, YearMonth.MonthValue.April, 30), new Date(2017, YearMonth.MonthValue.April, 29) },
                        {   new ByWeekday {Weekday = IsoDayOfWeek.Sunday, Clock = fakeClock}, IsoDayOfWeek.Sunday, new Date(2016, YearMonth.MonthValue.May, 01), new Date(2017, YearMonth.MonthValue.April, 30) },
                    })
                    .BDDfy();
            }

            public void GivenARepeatingDay(ByWeekday repeatingDay)
            {
                _repeatingDay = repeatingDay;
            }

            public void ThenAllDatesShouldBeThisDay(IsoDayOfWeek dayOfWeek)
            {
                _repeatingDay.GenerateDates()
                    .Where(r => r.IsoDayOfWeek != dayOfWeek)
                    .ShouldBeEmpty();
            }

            public void AndThenTheFirstDateShouldBeThis(Date firstDate)
            {
                _repeatingDay.GenerateDates()
                    .Min()
                    .ShouldBe(firstDate);
            }

            public void AndThenTheLastDateShouldBeThis(Date lastDate)
            {
                _repeatingDay.GenerateDates()
                    .Max()
                    .ShouldBe(lastDate);
            }
        }
    }
}