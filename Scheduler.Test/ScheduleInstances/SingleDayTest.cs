using System.Collections.Generic;
using System.Linq;
using Shouldly;
using NodaTime;
using TestStack.BDDfy;
using Scheduler.ScheduleInstances;
using Xunit;

namespace Scheduler.Test.SingleDayTests
{
    public class VerifyDay
    {
        SingleDay _sut;
        IEnumerable<LocalDate> _dates;

        [Fact]
        public void Execute()
        {
            this.WithExamples(new ExampleTable("sut", "expectedDate")
                {
                    {   new SingleDay() { Date = new LocalDate(2016, 04, 25) }, new LocalDate(2016, 04, 25) },
                    {   new SingleDay() { Date = new LocalDate(2000, 01, 01) }, new LocalDate(2000, 01, 01) },
                    {   new SingleDay() { Date = new LocalDate(2999, 12, 31) }, new LocalDate(2999, 12, 31) },
                })
                .BDDfy();
        }

        public void GivenASingleDay(SingleDay sut)
        {
            _sut = sut;
        }

        public void WhenDatesAreRetrieved()
        {
            _dates = _sut.Dates();
        }

        public void ThenOnlyThisDateIsReturned(LocalDate expectedDate)
        {
            _dates.Count().ShouldBe(1);

            _dates.Single().ShouldBe(expectedDate);
        }
    }
}
