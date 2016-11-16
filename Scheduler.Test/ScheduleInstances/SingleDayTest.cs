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
        IEnumerable<LocalDate> _occurrances;

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

        public void WhenOccurrancesAreRetrieved()
        {
            _occurrances = _sut.Occurrences();
        }

        public void ThenOnlyThisDateIsReturned(LocalDate expectedDate)
        {
            _occurrances.Count().ShouldBe(1);

            _occurrances.Single().ShouldBe(expectedDate);
        }
    }
}
