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
                        {   new ByWeekday {Weekday = IsoDayOfWeek.Monday, Clock = fakeClock}, IsoDayOfWeek.Monday, DateTimeHelper.GetLocalDate(2016, YearMonth.MonthValue.April, 25), DateTimeHelper.GetLocalDate(2017, YearMonth.MonthValue.April, 24) },
                        {   new ByWeekday {Weekday = IsoDayOfWeek.Tuesday, Clock = fakeClock}, IsoDayOfWeek.Tuesday, DateTimeHelper.GetLocalDate(2016, YearMonth.MonthValue.April, 26), DateTimeHelper.GetLocalDate(2017, YearMonth.MonthValue.April, 25) },
                        {   new ByWeekday {Weekday = IsoDayOfWeek.Wednesday, Clock = fakeClock}, IsoDayOfWeek.Wednesday, DateTimeHelper.GetLocalDate(2016, YearMonth.MonthValue.April, 27), DateTimeHelper.GetLocalDate(2017, YearMonth.MonthValue.April, 26) },
                        {   new ByWeekday {Weekday = IsoDayOfWeek.Thursday, Clock = fakeClock}, IsoDayOfWeek.Thursday, DateTimeHelper.GetLocalDate(2016, YearMonth.MonthValue.April, 28), DateTimeHelper.GetLocalDate(2017, YearMonth.MonthValue.April, 27) },
                        {   new ByWeekday {Weekday = IsoDayOfWeek.Friday, Clock = fakeClock}, IsoDayOfWeek.Friday, DateTimeHelper.GetLocalDate(2016, YearMonth.MonthValue.April, 29), DateTimeHelper.GetLocalDate(2017, YearMonth.MonthValue.April, 28) },
                        {   new ByWeekday {Weekday = IsoDayOfWeek.Saturday, Clock = fakeClock}, IsoDayOfWeek.Saturday, DateTimeHelper.GetLocalDate(2016, YearMonth.MonthValue.April, 30), DateTimeHelper.GetLocalDate(2017, YearMonth.MonthValue.April, 29) },
                        {   new ByWeekday {Weekday = IsoDayOfWeek.Sunday, Clock = fakeClock}, IsoDayOfWeek.Sunday, DateTimeHelper.GetLocalDate(2016, YearMonth.MonthValue.May, 01), DateTimeHelper.GetLocalDate(2017, YearMonth.MonthValue.April, 30) },
                    })
                    .BDDfy();
            }

            public void GivenARepeatingDay(ByWeekday repeatingDay)
            {
                _repeatingDay = repeatingDay;
            }

            public void ThenAllDatesShouldBeThisDay(IsoDayOfWeek dayOfWeek)
            {
                _repeatingDay.Dates()
                    .Where(r => r.IsoDayOfWeek != dayOfWeek)
                    .ShouldBeEmpty();
            }

            public void AndThenTheFirstDateShouldBeThis(LocalDate firstDate)
            {
                _repeatingDay.Dates()
                    .Min()
                    .ShouldBe(firstDate);
            }

            public void AndThenTheLastDateShouldBeThis(LocalDate lastDate)
            {
                _repeatingDay.Dates()
                    .Max()
                    .ShouldBe(lastDate);
            }
        }
    }
}