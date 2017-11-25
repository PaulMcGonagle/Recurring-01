using System.Collections.Generic;
using System.Linq;
using NodaTime;
using NodaTime.Testing;
using Scheduler.ScheduleInstances;
using Shouldly;
using TestStack.BDDfy;
using Xunit;

namespace Scheduler.Test.ScheduleInstances
{
    public class VerifyDay
    {
        private SingleDay _sut;
        private IClock _clock;
        private IEnumerable<IDate> _dates;

        [Fact]
        public void Execute()
        {
            var clock = new FakeClock(Instant.FromUtc(2016, 02, 10, 15, 40, 10));

            this.WithExamples(new ExampleTable("sut", "clock", "expectedDate")
                {
                    {   new SingleDay { Date = new Date(2016, YearMonth.MonthValue.April, 25) }, clock, new Date(2016, YearMonth.MonthValue.April, 25) },
                    {   new SingleDay { Date = new Date(2000, YearMonth.MonthValue.January, 01) }, clock, new Date(2000, YearMonth.MonthValue.January, 01) },
                    {   new SingleDay { Date = new Date(2999, YearMonth.MonthValue.December, 31) }, clock, new Date(2999, YearMonth.MonthValue.December, 31) },
                })
                .BDDfy();
        }

        public void GivenASingleDay(SingleDay sut)
        {
            _sut = sut;
        }

        public void WhenDatesAreRetrieved()
        {
            _dates = _sut.Generate(_clock);
        }

        public void ThenOnlyThisDateIsReturned(Date expectedDate)
        {
            _dates.Count().ShouldBe(1);

            _dates.Single().Value.ShouldBe(expectedDate.Value);
        }
    }
}
