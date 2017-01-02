﻿using System.Collections.Generic;
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
        IEnumerable<Date> _dates;

        [Fact]
        public void Execute()
        {
            this.WithExamples(new ExampleTable("sut", "expectedDate")
                {
                    {   new SingleDay { Date = new Date(2016, YearMonth.MonthValue.April, 25) }, new Date(2016, YearMonth.MonthValue.April, 25) },
                    {   new SingleDay { Date = new Date(2000, YearMonth.MonthValue.January, 01) }, new Date(2000, YearMonth.MonthValue.January, 01) },
                    {   new SingleDay { Date = new Date(2999, YearMonth.MonthValue.December, 31) }, new Date(2999, YearMonth.MonthValue.December, 31) },
                })
                .BDDfy();
        }

        public void GivenASingleDay(SingleDay sut)
        {
            _sut = sut;
        }

        public void WhenDatesAreRetrieved()
        {
            _dates = _sut.GenerateDates();
        }

        public void ThenOnlyThisDateIsReturned(Date expectedDate)
        {
            _dates.Count().ShouldBe(1);

            _dates.Single().Value.ShouldBe(expectedDate.Value);
        }
    }
}
