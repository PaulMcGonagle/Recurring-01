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
        public class VerifyOccurrences
        {
            private ByWeekday _repeatingDay;

            [Fact]
            public void Execute()
            {
                var fakeClock = ScheduleTestHelper.GetFakeClock(2016, 05, 01);

                this.WithExamples(table: new ExampleTable("repeatingDay", "dayOfWeek", "firstDate", "lastDate")
                    {
                        {   new ByWeekday() {Weekday = IsoDayOfWeek.Monday, Clock = fakeClock}, IsoDayOfWeek.Monday, new LocalDate(2016, 04, 25), new LocalDate(2017, 04, 24) },
                        {   new ByWeekday() {Weekday = IsoDayOfWeek.Tuesday, Clock = fakeClock}, IsoDayOfWeek.Tuesday, new LocalDate(2016, 04, 26), new LocalDate(2017, 04, 25) },
                        {   new ByWeekday() {Weekday = IsoDayOfWeek.Wednesday, Clock = fakeClock}, IsoDayOfWeek.Wednesday, new LocalDate(2016, 04, 27), new LocalDate(2017, 04, 26) },
                        {   new ByWeekday() {Weekday = IsoDayOfWeek.Thursday, Clock = fakeClock}, IsoDayOfWeek.Thursday, new LocalDate(2016, 04, 28), new LocalDate(2017, 04, 27) },
                        {   new ByWeekday() {Weekday = IsoDayOfWeek.Friday, Clock = fakeClock}, IsoDayOfWeek.Friday, new LocalDate(2016, 04, 29), new LocalDate(2017, 04, 28) },
                        {   new ByWeekday() {Weekday = IsoDayOfWeek.Saturday, Clock = fakeClock}, IsoDayOfWeek.Saturday, new LocalDate(2016, 04, 30), new LocalDate(2017, 04, 29) },
                        {   new ByWeekday() {Weekday = IsoDayOfWeek.Sunday, Clock = fakeClock}, IsoDayOfWeek.Sunday, new LocalDate(2016, 05, 01), new LocalDate(2017, 04, 30) },
                    })
                    .BDDfy();
            }

            public void GivenARepeatingDay(ByWeekday repeatingDay)
            {
                _repeatingDay = repeatingDay;
            }

            public void ThenAllOccurrencesShouldBeThisDay(IsoDayOfWeek dayOfWeek)
            {
                _repeatingDay.Dates()
                    .Where(o => o.IsoDayOfWeek != dayOfWeek)
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