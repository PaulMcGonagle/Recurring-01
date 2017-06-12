﻿using System.Collections.Generic;
using System.Linq;
using NodaTime;
using NodaTime.Testing;
using Scheduler.Generation;
using Shouldly;
using TestStack.BDDfy;
using Scheduler.ScheduleInstances;
using Xunit;

namespace Scheduler.Test.ScheduleInstances
{
    public class DateListTests
    {
        public class VerifyDay
        {
            private DateList _sut;
            private IClock _clock;
            private IEnumerable<IDate> _dates;

            [Fact]
            public void Execute()
            {
                var fakeClock = ScheduleTestHelper.GetFakeClock(2016, 02, 10, 15, 40, 10);

                this.WithExamples(new ExampleTable("sut", "clock", "expectedDates")
                    {
                        {
                            new DateList
                            {
                                Items = new List<IDate>
                                {
                                    new Date(2015, YearMonth.MonthValue.March, 17),
                                    new Date(2016, YearMonth.MonthValue.April, 25)
                                }
                            }, fakeClock,
                            new List<IDate>
                            {
                                {
                                    new Date(2015, YearMonth.MonthValue.March, 17)
                                },
                                {
                                    new Date(2016, YearMonth.MonthValue.April, 25)
                                }
                            }
                        }
                    })
                    .BDDfy();
            }

            public void GivenADateList(DateList sut)
            {
                _sut = sut;
            }

            public void AndGivenAClock(IClock clock)
            {
                _clock = clock;
            }

            public void WhenDatesAreRetrieved()
            {
                _dates = _sut.Generate(_clock);
            }

            public void ThenOnlyTheseDateAreReturned(IEnumerable<IDate> expectedDates)
            {
                _dates.Select(d => d.Value).ShouldBe(expectedDates.Select(e => e.Value));
            }
        }
    }
}