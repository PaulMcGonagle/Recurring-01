﻿using System.Linq;
using Shouldly;
using NodaTime;
using Scheduler;
using Scheduler.Persistance;
using Scheduler.Ranges;
using Scheduler.ScheduleEdges;
using Scheduler.ScheduleInstances;
using Scheduler.Test;
using TestStack.BDDfy;
using Xunit;
using Event = Scheduler.Event;

namespace Calendar.Test
{
    public class EventTests
    {
        public class CreateAndValidateEvent
        {
            private IEvent _sut;
            private IEdgeVertexs<ISerial> _serials;
            private IClock _clock;

            private const string TimeZoneProvider = "Europe/London";

            [Fact]
            public void Execute()
            {
                var fakeClock = ScheduleTestHelper.GetFakeClock(2016, 05, 01);

                this.WithExamples(new ExampleTable("sut", "clock", "expectedWeekday", "expectedStartTime", "expectedEndTime")
                    {
                        {
                            new Event
                            {
                                Serials = new EdgeVertexs<ISerial>(
                                    toVertex: new Serial(
                                                schedule: new ByWeekday(
                                                    weekday: IsoDayOfWeek.Thursday),
                                                rangeTime: new EdgeRangeTime(new RangeTime(new LocalTime(16, 45), new PeriodBuilder { Minutes = 45}.Build())),
                                                timeZoneProvider: TimeZoneProvider)),
                                Title = "Street dance",
                            },
                            fakeClock,
                            IsoDayOfWeek.Thursday,
                            new LocalTime(16, 45),
                            new LocalTime(17, 30)
                        }
                    })
                    .BDDfy();
            }

            public void GivenSut(Event sut)
            {
                _sut = sut;
            }

            public void WhenSomethingIsDone()
            {
                _serials = _sut.Serials;
            }

            public void ThenAllSerialsHaveTheCorrectWeekday(IsoDayOfWeek expectedWeekday)
            {
                _serials.SelectMany(s => s.ToVertex.GenerateEpisodes(_clock))
                    .Select(e => e.ToVertex.Start.DayOfWeek)
                    .ShouldAllBe(d => d.Equals((int)expectedWeekday));
            }

            public void AndThenAllStartTimesAreCorrect(LocalTime expectedStartTime)
            {
                _serials.SelectMany(s => s.ToVertex.GenerateEpisodes(_clock))
                    .Select(e => e.ToVertex.Start.TimeOfDay)
                    .ShouldAllBe(d => d.Equals(expectedStartTime));
            }

            public void AndThenAllEndTimesAreCorrect(LocalTime expectedEndTime)
            {
                _serials.SelectMany(s => s.ToVertex.GenerateEpisodes(_clock))
                    .Select(e => e.ToVertex.To.TimeOfDay)
                    .ShouldAllBe(d => d.Equals(expectedEndTime));
            }
        }
        public class VerifyTimeInOtherTimeZone
        {
            private IEvent _sut;
            private IClock _clock;
            private IEdgeVertexs<ISerial> _serials;

            private const string TimeZoneProvider = "Europe/London";

            [Fact]
            public void Execute()
            {
                var fakeClock = ScheduleTestHelper.GetFakeClock(2016, 05, 01);

                this.WithExamples(new ExampleTable("sut", "clock", "expectedStartTime", "expectedEndTime")
                    {
                        {
                            new Event
                            {
                                Serials = 
                                    new EdgeVertexs<ISerial>(new Serial(
                                            schedule: new SingleDay
                                            {
                                                Date = new Scheduler.Date(2016, YearMonth.MonthValue.July, 01),
                                            },
                                            rangeTime: new EdgeRangeTime(new LocalTime(14, 00), new PeriodBuilder { Minutes = 1 }.Build()),
                                            timeZoneProvider: "Europe/London")),
                                Title = "Street dance",
                            },
                            fakeClock,
                            new LocalTime(14, 00),
                            new LocalTime(14, 45)
                        }
                    })
                    .BDDfy();
            }

            public void GivenSut(Event sut)
            {
                _sut = sut;
            }

            public void WhenSomethingIsDone()
            {
                _serials = _sut.Serials;
            }

            public void ThenAllStartTimesAreCorrect(LocalTime expectedStartTime)
            {
                _serials.SelectMany(s => s.ToVertex.GenerateEpisodes(_clock))
                    .Select(e => e.ToVertex.Start.TimeOfDay)
                    .ShouldAllBe(d => d.Equals(expectedStartTime));
            }

            public void AndThenAllEndTimesAreCorrect(LocalTime expectedEndTime)
            {
            }
        }
    }
}
